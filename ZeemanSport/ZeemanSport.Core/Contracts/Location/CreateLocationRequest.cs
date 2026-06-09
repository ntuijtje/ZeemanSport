using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Location;

namespace ZeemanSport.Core.Contracts.Location
{
    public class CreateLocationRequest
    {
        public string? Name { get; set; }
        public LocationType LocationType { get; set; }
        public int Capacity { get; set; }
        public int WidthInSeats { get; set; }
        public int HeightInSeats { get; set; }
    }
}
