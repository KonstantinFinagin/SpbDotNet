﻿namespace SpbDotNet.Approvals.Tests.Helpers;

public static class DefaultJsonOptions
{
    static DefaultJsonOptions()
    {
        Indented = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }
    public static JsonSerializerOptions Indented { get; }
}
