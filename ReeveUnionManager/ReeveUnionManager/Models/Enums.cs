using System;

namespace ReeveUnionManager.Models;

public enum CallLogError
{
    None,
    InsertionError,
    NameTooShort,
    DuplicateCallId,
    MissingDate
}

public enum CheckInLogError
{
    None,
    InsertionError,
    DuplicateCheckInId,
    NameTooShort,
    MissingDate
}
