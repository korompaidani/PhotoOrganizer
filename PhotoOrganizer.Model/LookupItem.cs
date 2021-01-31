﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoOrganizer.Model
{
    public class LookupItem
    {
        public int Id { get; set; }
        public string DisplayMemberItem { get; set; }
    }

    public class NullLookupItem : LookupItem
    {
        public new int? Id { get { return null; } }
    }
}
