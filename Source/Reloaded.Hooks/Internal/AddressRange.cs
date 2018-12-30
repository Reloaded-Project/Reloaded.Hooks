namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// Defines a physical address range with a minimum and maximum address.
    /// </summary>
    public struct AddressRange
    {
        public long StartPointer;
        public long EndPointer;

        public AddressRange(long startPointer, long endPointer)
        {
            StartPointer = startPointer;
            EndPointer = endPointer;
        }

        /// <summary>
        /// Returns true if a specified point is contained in this address range.
        /// </summary>
        public bool Contains(long point)
        {
            return PointInRange(ref this, point);
        }

        /// <summary>
        /// Returns true if a number "point", is between min and max of address range.
        /// </summary>
        private bool PointInRange(ref AddressRange range, long point)
        {
            if (point >= range.StartPointer &&
                point <= range.EndPointer)
                return true;

            return false;
        }
    }
}
