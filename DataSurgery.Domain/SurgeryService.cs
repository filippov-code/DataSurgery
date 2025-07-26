using DataSurgery.Core.Interfaces;
using DataSurgery.Core;
using DataSurgery.Core.Enums;
using DataSurgery.Domain.Enums;
using DataSurgery.Encryption;
using System.Security.Cryptography;
using System.Text;

namespace DataSurgery.Domain;

public class SurgeryService
{
    public void Hide(
        string containerPath,
        string messagePath,
        string savePath,
        int degree,
        string password,
        string iv,
        SurgeryMethods method,
        Action<HideProcessSteps>? processHandler)
    {
        processHandler?.Invoke(HideProcessSteps.DataCollection);
        string containerExtension = Path.GetExtension(containerPath);
        string messageExtension = Path.GetExtension(messagePath);
        byte[] messageBytes = File.ReadAllBytes(messagePath);

        processHandler?.Invoke(HideProcessSteps.MetatagCreating);
        var aes = new AES(password, iv);
        var fpe = new FPE(aes);
        byte[] messageEncrypted = aes.Encrypt(messageBytes);

        var dsmd = new DSMD(
            messageExtension,
            (int)method,
            degree,
            messageEncrypted.Length,
            MD5.HashData(messageBytes)
        );
        byte[] dsmdABlock = dsmd.GetABlock();

        processHandler?.Invoke(HideProcessSteps.Encryption);
        byte[] dsmdEncrypted = fpe.PrefixEncrypt(dsmdABlock);
        byte[] extensionEncrypted = fpe.PrefixEncrypt(Encoding.UTF8.GetBytes(messageExtension));
        var surgery = SurgeryFactory.GetSurgery(containerPath);
        surgery.AddLSB(dsmdEncrypted, 1);

        processHandler?.Invoke(HideProcessSteps.Hiding);
        byte[] toWrite = extensionEncrypted.Concat(messageEncrypted).ToArray();
        surgery.AddLSB(toWrite, degree);

        processHandler?.Invoke(HideProcessSteps.Saving);
        surgery.Save(savePath + containerExtension);

        processHandler?.Invoke(HideProcessSteps.Done);
    }

    public void Find(
        string stegoContainerPath,
        string savePath,
        string password,
        string iv,
        Action<FindProcessSteps>? processHandler)
    {
        processHandler?.Invoke(FindProcessSteps.DataCollection);
        byte[] stegoContainerBytes = File.ReadAllBytes(stegoContainerPath);

        processHandler?.Invoke(FindProcessSteps.DecryptionPreparing);
        var aes = new AES(password, iv);
        var fpe = new FPE(aes);
        var surgery = SurgeryFactory.GetSurgery(stegoContainerPath);
        var dsmdEncrypted = surgery.FindLSB(DSMD.Size, 1, 0);

        processHandler?.Invoke(FindProcessSteps.Decryption);
        var dsmdDecrypted = fpe.PrefixDecrypt(dsmdEncrypted);

        processHandler?.Invoke(FindProcessSteps.MetatagParsing);
        var dsmd = new DSMD(dsmdDecrypted);
        byte[] encryptedExtension = surgery.FindLSB(dsmd.ExtensionSize, dsmd.Degree, DSMD.Size * 8);
        byte[] decryptedExtension = fpe.PrefixDecrypt(encryptedExtension);
        string extension = DSMD.Encoding.GetString(decryptedExtension);

        processHandler?.Invoke(FindProcessSteps.MessageExtracting);
        byte[] encryptedMessage = surgery.FindLSB(dsmd.MessageSize, dsmd.Degree, DSMD.Size * 8 + dsmd.ExtensionSize * 8 / dsmd.Degree);
        byte[] message = aes.Decrypt(encryptedMessage);

        processHandler?.Invoke(FindProcessSteps.Saving);
        File.WriteAllBytes(savePath + "." + extension, message);

        processHandler?.Invoke(FindProcessSteps.Done);
    }
}
