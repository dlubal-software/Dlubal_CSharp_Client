#if RFEM
#elif RSTAB

#endif

namespace Steel_Hall_GUI
{
    public class Bracing
    {
        public int BracingType { get; set; }
        public int BracingNumber { get; set; }
        public int LoopCount { get; set; }
        public int Increment { get; set; }
    }
}