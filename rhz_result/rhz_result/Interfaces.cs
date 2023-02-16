namespace rhz_result;


/// <summary>
/// Result wrapper interface that either holds a Value or an Error, depending on Error State
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResult<T> : IErrorable, IValue<T> { }


/// <summary>
/// Represents an IResult of ReadonlyCollection of T and is a QoL wrapper to directly enumerate on IEnumerableResult without having to call result.Value
/// </summary>
/// <typeparam name="T">Type of Item in IReadonlyCollection</typeparam>
public interface IEnumerableResult<T> : IReadOnlyCollection<T>, IResult<IReadOnlyCollection<T>> { }


/// <summary>
/// object can go into Error State
/// </summary>
public interface IErrorable {

    /// <summary>
    /// object is not in Error State
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// object is in Error State
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// the error the object can return
    /// </summary>
    public Exception Error { get; }
}

/// <summary>
/// generic Value wrapper
/// </summary>
/// <typeparam name="T">Type of Value</typeparam>
public interface IValue<T> {

    /// <summary>
    /// gets the wrapped Value
    /// </summary>
    public T Value { get; }
}
