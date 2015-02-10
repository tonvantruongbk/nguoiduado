using System;
using System.Collections.Generic;

namespace nguoiduado.Models
{
    public class ListModel<T> where T : class
    {
        public Decimal TotalRecord { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public IEnumerable<T> ListDiplay { get; set; }
        public Decimal MaNoiDung { get; set; }

        //Dùng để tìm kiếm
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        //Dùng cho Văn Bản
        public int ID { get; set; }
        public IEnumerable<T> ListDiplayVanBan { get; set; }
        public string TenVanBan { get; set; }
        public string TenMenu { get; set; }

    }
}