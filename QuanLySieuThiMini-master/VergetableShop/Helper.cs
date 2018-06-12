using BookShop.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookShop
{
    public static class Helper
    {

        #region Database
        public static VergetableContext db = new VergetableContext();

        public static void Reload()
        {
            try
            {
                var context = ((IObjectContextAdapter)db).ObjectContext;
                var refreshableObjects = (from entry in context.ObjectStateManager.GetObjectStateEntries(
                                                           EntityState.Added
                                                           | EntityState.Deleted
                                                           | EntityState.Modified
                                                           | EntityState.Unchanged)
                                          where entry.EntityKey != null
                                          select entry.Entity).ToList();

                context.Refresh(RefreshMode.StoreWins, refreshableObjects);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }
        #endregion

        #region Hàm chức năng

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        #endregion

        public static string TenSanPham(MATHANG a)
        {
            string ans = "";

            ans = a.TEN;

            return ans;
        }

        public static int SoLuong(MATHANG a, DateTime batdau, DateTime ketthuc)
        {
            int ans = (
                        from chitiet in db.CHITIETHOADONs.Where(p => p.MATHANGID == a.ID).ToList()
                        from hoadon in db.HOADONs.Where(p =>p.ID == chitiet.HOADONID && p.NGAY >= batdau && p.NGAY <= ketthuc).ToList()
                        select chitiet
                      )
                      .ToList()
                      .Sum(p => (int)p.SOLUONG);

            return ans;
        }

        public static int GiaSanPham(MATHANG a)
        {
            int ans = 0;

            ans =  (int)a.GIABAN;
            var listThue = db.THUE_MATHANG.Where(x => x.ID_MATHANG == a.ID).ToList();
            foreach(var item in listThue)
            {
                ans +=(int)a.GIABAN * (int)db.THUEs.SingleOrDefault(x => x.ID == item.ID_THUE).ThueXuat / 100;
            }

            return ans;
        }
        public static int GiaNhap(MATHANG a)
        {
            int ans = 0;
            ans = (int)a.GIASI;
            var listThue = db.THUE_MATHANG.Where(x => x.ID_MATHANG == a.ID).ToList();
            foreach (var item in listThue)
            {
                ans += (int)a.GIABAN * (int)db.THUEs.SingleOrDefault(x => x.ID == item.ID_THUE).ThueXuat / 100;
            }
            return ans;
        }
        public static int SoLuongNhap(MATHANG a,DateTime _start ,DateTime _end)
        {
            int ans = (
                        from chitiet in db.CHITIETNHAPs.Where(p => p.MATHANGID == a.ID).ToList()
                        from phieunhap in db.PHIEUNHAPs.Where(p => p.ID == chitiet.PHIEUNHAPID && p.NGAY >= _start && p.NGAY <= _end).ToList()
                        select chitiet
                        ).ToList()
                        .Sum(p =>(int)p.SOLUONG);
            return ans;
        }
        public static int SoLuongALL(MATHANG a)
        {
            int ans = (
                        from chitiet in db.CHITIETNHAPs.Where(p => p.MATHANGID == a.ID).ToList()
                        from phieunhap in db.PHIEUNHAPs.Where(p => p.ID == chitiet.PHIEUNHAPID).ToList()
                        select chitiet
                        ).ToList()
                        .Sum(p => (int)p.SOLUONG);
            return ans;
        }
        public static int IDSanPham = 0; // sản phẩm lựa chọn
        public static int IDPhieuNhap = 0; //  Phiếu nhập đang nhập
        public static int IDHoaDon = 0; // Hóa đơn đang bán
        public static int IDNhanVien = 0; // Nhân viên đang đăng nhập

    }
}
