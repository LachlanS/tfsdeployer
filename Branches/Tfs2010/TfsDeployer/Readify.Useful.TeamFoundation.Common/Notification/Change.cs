using System;

namespace Readify.Useful.TeamFoundation.Common.Notification
{
   
    [Serializable]
    public class Change
    {
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }

}
