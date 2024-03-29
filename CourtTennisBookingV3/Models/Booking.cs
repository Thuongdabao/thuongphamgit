﻿using System;
using System.Collections.Generic;

#nullable disable

namespace CourtTennisBookingV3.Models
{
    public partial class Booking
    {
        public string Id { get; set; }
        public string CusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public double? Price { get; set; }
        public string CourtId { get; set; }
        public DateTime? BookingDate { get; set; }
        public bool? Status { get; set; }
        public string CusName { get; set; }

        public virtual TennisCourt Court { get; set; }
        public virtual Customer Cus { get; set; }
    }
}
