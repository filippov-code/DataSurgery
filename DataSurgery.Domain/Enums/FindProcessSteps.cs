namespace DataSurgery.Domain.Enums;

public enum FindProcessSteps
{
    DataCollection = 1,
    DecryptionPreparing = 2,
    Decryption = 3,
    MetatagParsing = 4,
    MessageExtracting = 5,
    Saving = 6, 
    Done = 7
}
