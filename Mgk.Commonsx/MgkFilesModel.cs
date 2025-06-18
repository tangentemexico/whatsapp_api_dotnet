using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Commonsx
{
    /// <summary>
    /// Clase modelo para manejo de archivos.
    /// agustinistmo@gmail.com
    /// </summary>
    public class MgkFilesModel
    {
        public String TrgtPath { get; set; }
        public String FileName { get; set; }
        public String FileExt { get; set; }
        public String FileType { get; set; }
        public byte[] _Bytes { get; set; }
        public System.IO.Stream _Stream { get; set; }

        public Boolean? _Exist { get; set; }
        public String _Message { get; set; }

        public String _Bytess { get; set; }

        public Boolean Exists()
        {
            return System.IO.File.Exists(this._FullName);
        }

        public String _FileName
        {
            get { return FileName + ((FileExt ?? "") != "" ? ("." + FileExt) : ""); }
            set
            {
                FileName = System.IO.Path.GetFileNameWithoutExtension(value);
                FileExt = (value.LastIndexOf('.')>=0)?System.IO.Path.GetExtension(value).Substring(1):"";
            }
        }

        public String _FullName
        {
            get { return ((TrgtPath != null && TrgtPath.Length > 0 && TrgtPath.Substring(TrgtPath.Length - 1, 1) != ("" + Path.DirectorySeparatorChar)) ? (TrgtPath + ("" + Path.DirectorySeparatorChar)) : TrgtPath)  + _FileName; }
            set
            {
                _FileName = value;
                TrgtPath = System.IO.Path.GetDirectoryName(value)+(""+ Path.DirectorySeparatorChar);
            }
        }
    }
}
