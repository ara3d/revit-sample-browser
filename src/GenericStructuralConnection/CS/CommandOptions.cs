// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.GenericStructuralConnection.CS
{
    /// <summary>
    ///     Enum for user commands.
    /// </summary>
    public enum CommandOption
    {
        /// <summary>
        ///     Create generic connection.
        /// </summary>
        CreateGeneric,

        /// <summary>
        ///     Delete generic connection.
        /// </summary>
        DeleteGeneric,

        /// <summary>
        ///     Read generic connection.
        /// </summary>
        ReadGeneric,

        /// <summary>
        ///     Update generic connection.
        /// </summary>
        UpdateGeneric,

        /// <summary>
        ///     Create detailed connection.
        /// </summary>
        CreateDetailed,

        /// <summary>
        ///     Change detailed connection.
        /// </summary>
        ChangeDetailed,

        /// <summary>
        ///     Copy detailed connection.
        /// </summary>
        CopyDetailed,

        /// <summary>
        ///     Match properties of detailed connections.
        /// </summary>
        MatchPropDetailed,

        /// <summary>
        ///     Reset detailed connection.
        /// </summary>
        ResetDetailed
    }
}
