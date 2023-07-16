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

    public static Either<T1Out, T2> BindFirst<T1, T2, T1Out>(this Either<T1, T2> either, Func<T1, T1Out> primaryFunc) {
        if (either.HasFirst) return new Either<T1Out, T2>(primaryFunc(either.First), default);
        else return new Either<T1Out, T2>(default, either.Second);
    }
    public static Either<T1, T2Out> BindSecond<T1, T2, T2Out>(this Either<T1, T2> either, Func<T2, T2Out> primaryFunc) {
        if (either.HasSecond) return new Either<T1, T2Out>(default, primaryFunc(either.Second));
        else return new Either<T1, T2Out>(either.First, default);
    }

    public static Either<T1Out, T2Out> Bind<T1, T2, T1Out, T2Out>(this Either<T1, T2> either, 
        Func<T1, T1Out> primaryFunc, 
        Func<T2, T2Out> secondaryFunc) {
            if(either.HasFirst) return new Either<T1Out, T2Out>(primaryFunc(either.First), default);
            else return new Either<T1Out, T2Out>(default, secondaryFunc(either.Second));
    }

    public static Either<T1,T2> MatchFirst<T1, T2>(this Either<T1, T2> either, Action<T1> action) {
        if(either.HasFirst) action(either.First);
        return either;
    }
    public static Either<T1,T2> MatchSecond<T1, T2>(this Either<T1, T2> either, Action<T2> action) {
        if(either.HasSecond) action(either.Second);
        return either;
    }
    public static Either<T1, T2> Match<T1, T2>(this Either<T1, T2> either, Action<T1> leftMatch, Action<T2> rightMatch) {
        if(either.HasFirst) leftMatch(either.First);
        else if( either.HasSecond) rightMatch(either.Second);
        return either;
    }
}
