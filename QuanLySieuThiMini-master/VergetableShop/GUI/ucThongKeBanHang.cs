using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BookShop.Model;
using System.Windows.Forms.DataVisualization.Charting;

namespace BookShop.GUI
{
    public partial class ucThongKeBanHang : UserControl
    {
        private VergetableContext db = Helper.db;

        #region Hàm khởi tạo
        public ucThongKeBanHang()
        {
            InitializeComponent();
            Helper.Reload();
        }
        #endregion

        #region LoadForm
        private void Loaddgv()
        {
            int i = 0;
            var listMatHang = (
                                     from mathang in db.MATHANGs.ToList()
                                     select new
                                     {
                                         MatHang = Helper.TenSanPham(mathang),
                                         DonGia = Helper.GiaSanPham(mathang),
                                         SoLuong = Helper.SoLuong(mathang, dateBatDau.DateTime, dateKetThuc.DateTime),
                                         ThanhTien = ((int)Helper.GiaSanPham(mathang) * Helper.SoLuong(mathang, dateBatDau.DateTime, dateKetThuc.DateTime)).ToString("N0"),
                                         gt = ((int)Helper.GiaSanPham(mathang) * Helper.SoLuong(mathang, dateBatDau.DateTime, dateKetThuc.DateTime))
                                     })
                                     .ToList()
                                     .OrderByDescending(p => p.ThanhTien)
                                     .Select(p => new
                                     {
                                         STT = ++i,
                                         MatHang = p.MatHang,
                                         DonGia = p.DonGia.ToString("N0"),
                                         SoLuong = p.SoLuong,
                                         ThanhTien = p.ThanhTien,
                                         gt = p.gt
                                     })
                                     .ToList();

            dgvMatHangMain.DataSource = listMatHang;

            chartThongKe.DataSource = listMatHang.Where(p => p.gt > 0).ToList();
            chartThongKe.Series[0].XValueMember = "MatHang";
            chartThongKe.Series[0].XValueType = ChartValueType.String;
            chartThongKe.Series[0].YValueMembers = "ThanhTien";
            chartThongKe.Series[0].YValueType = ChartValueType.Int32;


            // Thống kê giá trị nhập hàng
            int j = 0;
            var listNhap = (from mathang in db.MATHANGs.ToList()
                            select new
                            {
                                MatHang = Helper.TenSanPham(mathang),
                                GiaNhap = Helper.GiaNhap(mathang),
                                SoLuong = Helper.SoLuongNhap(mathang,dateBatDau.DateTime,dateKetThuc.DateTime),
                                ThanhTien = (Helper.GiaNhap(mathang)*Helper.SoLuongNhap(mathang, dateBatDau.DateTime, dateKetThuc.DateTime)).ToString("N0"),
                                gt = Helper.GiaNhap(mathang) * Helper.SoLuongNhap(mathang, dateBatDau.DateTime, dateKetThuc.DateTime)
                            })
                            .ToList()
                            .OrderByDescending(p=>p.ThanhTien)
                            .Select(p=> new {
                                STT = ++j,
                                MatHang = p.MatHang,
                                GiaNhap = p.GiaNhap.ToString("N0"),
                                SoLuong = p.SoLuong,
                                ThanhTien = p.ThanhTien,
                                gt = p.gt
                            }).ToList();
            gridControl1.DataSource = listNhap;

            chartNhap.DataSource = listNhap.Where(p => p.gt > 0).ToList();
            chartNhap.Series[0].XValueMember = "MatHang";
            chartNhap.Series[0].XValueType = ChartValueType.String;
            chartNhap.Series[0].YValueMembers = "gt";
            chartNhap.Series[0].YValueType = ChartValueType.Int32;

            int Tong = listMatHang.Sum(p => p.gt) - listNhap.Sum(p => p.gt);
            label_Tong.Text = Tong.ToString("N0");
        }
        private void ucThongKeBanHang_Load(object sender, EventArgs e)
        {
            dateBatDau.DateTime = DateTime.Now.AddDays(-30);
            dateKetThuc.DateTime = DateTime.Now;
            Loaddgv();
        }
        #endregion

        #region Sự kiện ngầm
        private void dateKetThuc_EditValueChanged(object sender, EventArgs e)
        {
            Loaddgv();
        }
        #endregion
    }
}
