// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.PowerCircuit.CS
{
    /// <summary>
    ///     An enumerate type listing the operations
    /// </summary>
    public enum Operation
    {
        /// <summary>
        ///     Create a new electrical circuit
        /// </summary>
        CreateCircuit,

        /// <summary>
        ///     Edit circuit
        /// </summary>
        EditCircuit,

        /// <summary>
        ///     Set a panel for circuit
        /// </summary>
        SelectPanel,

        /// <summary>
        ///     Disconnect the panel from circuit
        /// </summary>
        DisconnectPanel
    }

    /// <summary>
    ///     An enumerate type listing the options to edit a circuit
    /// </summary>
    public enum EditOption
    {
        /// <summary>
        ///     Add an element to the circuit
        /// </summary>
        Add,

        /// <summary>
        ///     Remove an element from the circuit
        /// </summary>
        Remove,

        /// <summary>
        ///     Set a panel for circuit
        /// </summary>
        SelectPanel
    }
}
