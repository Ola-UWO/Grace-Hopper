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

public enum CheckInLogError
{
    None,
    InsertionError,
    DuplicateCheckInId,
    NameTooShort,
    MissingDate,
    MissingLocation,
    DeleteError,
    CheckInLogIdNotFound
}

public enum BasicEntryError
{
    None,
    InsertionError
}
