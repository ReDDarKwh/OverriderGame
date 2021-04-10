using System;
using System.Collections.Generic;

static class EventRepo
{
    public static readonly Func<Dictionary<string, object>, bool> targetOutOfTargetList = (Dictionary<string, object> data) =>
    {
        return true;
    };
}