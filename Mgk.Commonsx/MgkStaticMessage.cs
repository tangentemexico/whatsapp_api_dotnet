using System;
using System.Collections.Generic;
using System.Text;

namespace Mgk.Commonsx
{
    public static class MgkStaticMessage
    {
        public static MgkMessage Message { get; set; } = new MgkMessage();
        public static void Clear()
        {
            Message.Clear();
        }
    }
}