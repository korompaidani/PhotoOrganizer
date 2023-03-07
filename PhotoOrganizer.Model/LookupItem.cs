using PhotoOrganizer.Common;

namespace PhotoOrganizer.Model
{
    public class LookupItem
    {
        public int Id { get; set; }
        public string DisplayMemberItem { get; set; }
        public string PhotoPath { get; set; }
        public string Coordinates { get; set; }
        public ColorSign ColorFlag { get; set; }
    }

    public class NullLookupItem : LookupItem
    {
        public new int? Id { get { return null; } }
    }
}