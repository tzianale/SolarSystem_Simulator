namespace Utils
{
    /// <summary>
    /// Class Two Object Container is a utility class that is used when a Method has to return two objects,
    /// possibly of different types, and an array / list is not seen as an optimal solution.
    /// </summary>
    /// 
    /// <typeparam name="TFirst">
    /// The type of the first Object contained in this class
    /// </typeparam>
    /// <typeparam name="TSecond">
    /// The type of the second Object contained in this class
    /// </typeparam>
    public class TwoObjectContainer<TFirst, TSecond>
    {
        public readonly TFirst FirstObject;
        public readonly TSecond SecondObject;

        /// <summary>
        /// Constructor for Two Object Containers, asks for the initial values of the contained objects 
        /// </summary>
        /// 
        /// <param name="firstObjectInitializer">
        /// Initial value for the first object
        /// </param>
        /// 
        /// <param name="secondObjectInitializer">
        /// Initial value for the second object
        /// </param>
        public TwoObjectContainer(TFirst firstObjectInitializer, TSecond secondObjectInitializer)
        {
            FirstObject = firstObjectInitializer;
            SecondObject = secondObjectInitializer;
        }
    }
}