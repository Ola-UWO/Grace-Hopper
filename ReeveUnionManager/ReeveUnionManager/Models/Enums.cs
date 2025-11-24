using System;

namespace ReeveUnionManager.Models;

public enum CallLogError
{
    None,
    InsertionError,
    NameTooShort,
    DuplicateCallId,
    MissingDate,
    DeleteError,
    CallLogIdNotFound
}

public enum BasicEntryError
{
    None,
    InsertionError
}

public enum ScrapeEventError
{
    None,
    InsertionError,
    DeleteError

}

public enum ManagerLogError
{
    
}
