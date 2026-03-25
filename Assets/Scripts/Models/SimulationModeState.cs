namespace Models
{
    /// <summary>
    /// Represents the state and modes of the simulation.
    /// </summary>
    public static class SimulationModeState
    {
        /// <summary>
        /// Defines the possible modes of the simulation.
        /// </summary>
        public enum SimulationMode
        {
            /// <summary>
            /// Explorer mode allows users to explore the simulation environment.
            /// </summary>
            Explorer,

            /// <summary>
            /// Sandbox mode allows users to manipulate and experiment with the simulation parameters freely.
            /// </summary>
            Sandbox
        }

        public static SimulationMode currentSimulationMode;
    }
}