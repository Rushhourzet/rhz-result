namespace rhz.Result;


/// <summary>
/// Result wrapper interface that either holds a Value or an Error, depending on Error State
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResult<T> {
    /// <summary>
    /// object is not in Error State
    /// </summary>
    public bool IsOk { get; }

    /// <summary>
    /// object is in Error State
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// the error the object can return. When used you should first Check with isError 
    /// </summary>
    public Exception Error { get; }

    /// <summary>
    /// gets the wrapped Value. When used you should first Check with isOk 
    /// </summary>
    public T Value { get; }
}
