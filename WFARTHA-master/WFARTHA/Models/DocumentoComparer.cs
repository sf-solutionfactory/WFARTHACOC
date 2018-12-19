using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFARTHA.Entities;

namespace WFARTHA.Models
{
    public class DocumentoComparer : IEqualityComparer<DOCUMENTO>
    {
        public bool Equals(DOCUMENTO x, DOCUMENTO y)
        {
            return x.NUM_DOC == y.NUM_DOC;
        }

        public int GetHashCode(DOCUMENTO obj)
        {
            return obj.NUM_DOC.GetHashCode();
        }
    }
}