using rhz.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace rhz.Fluent;

public readonly struct Either<T1, T2> {
    private readonly T1? first;
    private readonly T2? second;
    private readonly bool hasFirst;

    public Either(T1 first, T2 second) {
        if (first is not null) {
            this.first = first;
            this.second = default;
            hasFirst = true;
        }
        else if (second is not null) {
            this.first = default;
            this.second = second;
            hasFirst = false;
        }
        else throw new ArgumentException("Both first and second cannot be null");
    }
    public Either(Func<T1> left, Func<T2> right) : this(left(),right()) {}


    internal T1 First => hasFirst ? first! : throw new InvalidOperationException("Cannot get First when Either has second Value");
    internal T2 Second => !hasFirst ? second! : throw new InvalidOperationException("Cannot get Second when Either has first Value");

    internal bool HasFirst => hasFirst;

    internal bool HasSecond => !hasFirst;
}


public static class EitherExtension {
    public static Either<T1Out, T2Out> Bind<T1,T2, T1Out, T2Out>(
        this Either<T1, T2> either, 
        Func<T1, T1Out> primaryFunc, 
        Func<T2, T2Out> secondaryFunc) 
        => new Either<T1Out, T2Out>(primaryFunc(either.First), secondaryFunc(either.Second));

    public static void Match<TLeft, TRight>(this Either<TLeft, TRight> either,
        Action<TLeft> leftMatch,
        Action<TRight> rightMatch) {
        if(either.HasFirst) leftMatch(either.First);
        else rightMatch(either.Second);
    }
}
