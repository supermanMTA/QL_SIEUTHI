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
using BookShop.Report;

namespace BookShop.GUI
{
    public partial class ucNhapHang : UserControl
    {
        private VergetableContext db = Helper.db;

        #region Hàm khởi tạo
        public ucNhapHang()
        {
            InitializeComponent();
            Helper.Reload();

            dgvChiTietNhapMain.DataSource = null;
            dgvPhieuNhapMain.DataSource = null;
        }

        #endregion


        #region LoadForm
        private void LoadInitControl()
        {
            dateBatDau.DateTime = DateTime.Now.AddDays(-30);
            dateKetThuc.DateTime = DateTime.Now;
        }

        private void LoadDgvDanhSachHoaDon()
        {
            txtTongGiaTri.Text = "Tổng giá trị: 0";
            try
            {
                dgvPhieuNhapMain.DataSource = db.PHIEUNHAPs.ToList()
                                               .Where(p => p.NGAY >= dateBatDau.DateTime && p.NGAY <= dateKetThuc.DateTime)
                                               .OrderBy(p => p.NGAY)
                                               .Select(p => new
                                               {
                                                   ID = p.ID,
                                                   TongChiPhi = ((int)p.TONGTIEN).ToString("N0"),
                                                   MaHoaDon = p.MAPHIEUNHAP,
                                                   Ngay = ((DateTime)p.NGAY).ToString("dd/MM/yyyy"),
                                                   NhanVien = db.NHANVIENs.Where(z => z.ID == p.NHANVIENID).FirstOrDefault().TEN,
                                                   STATUS = p.STATUS == 0? "Chưa duyệt" : "Đã duyệt"
                                               })
                                               .Where(p=>p.MaHoaDon.ToUpper().Contains(txtTimKiem.Text.ToUpper()))
                                               .ToList();

                txtTongGiaTri.Text = "Tổng giá trị : " + ((int) db.PHIEUNHAPs.ToList()
                                                           .Where(p => p.NGAY >= dateBatDau.DateTime && p.NGAY <= dateKetThuc.DateTime)
                                                           .OrderBy(p => p.NGAY)
                                                           .Select(p => new
                                                           {
                                                               ID = p.ID,
                                                               ChiPhi = p.TONGTIEN,
                                                               TongChiPhi = ((int)p.TONGTIEN).ToString("N0"),
                                                               MaHoaDon = p.MAPHIEUNHAP,
                                                               Ngay = ((DateTime)p.NGAY).ToString("dd/MM/yyyy"),
                                                               NhanVien = db.NHANVIENs.Where(z => z.ID == p.NHANVIENID).FirstOrDefault().TEN
                                                           })
                                                           .Sum(p => p.ChiPhi))
                                                           .ToString("N0");

                LoadCHITIETNHAP();
            }
            catch { }
        }

        private void LoadCHITIETNHAP()
        {
            try
            {
                int ID = (int)dgvPhieuNhap.GetFocusedRowCellValue("ID");
                PHIEUNHAP hd = db.PHIEUNHAPs.Where(p => p.ID == ID).FirstOrDefault();

                int i = 0;
                dgvChiTietNhapMain.DataSource = db.CHITIETNHAPs.Where(p => p.PHIEUNHAPID == hd.ID).ToList()
                                                .Select(p => new
                                                {
                                                    ID = p.ID,
                                                    STT = ++i,
                                                    MatHang = Helper.TenSanPham(db.MATHANGs.Where(z => z.ID == p.MATHANGID).FirstOrDefault()),
                                                    DonGia = ((int)p.DONGIA).ToString("N0"),
                                                    SoLuong = p.SOLUONG,
                                                    ThanhTien = ((int)p.THANHTIEN).ToString("N0")
                                                })
                                                .ToList();
            }
            catch { }
        }

        private void ucBanHang_Load(object sender, EventArgs e)
        {
            LoadInitControl();
            LoadDgvDanhSachHoaDon();
        }
        #endregion

        #region Sự kiện ngầm
        private void dgvHoaDon_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateDetail();
            if(dgvPhieuNhap.GetFocusedRowCellValue("STATUS").ToString() == "Chưa duyệt")
            {
                btn_Duyet.Enabled = true;
                btn_Xoa.Enabled = true;
            }
        }

        private void dateKetThuc_EditValueChanged(object sender, EventArgs e)
        {
            LoadDgvDanhSachHoaDon();
        }
        #endregion

        #region Hàm chức năng

        private void UpdateDetail()
        {
            try
            {
                int ID = (int)dgvPhieuNhap.GetFocusedRowCellValue("ID");
                PHIEUNHAP hd = db.PHIEUNHAPs.Where(p => p.ID == ID).FirstOrDefault();

                txtMaPhieuNhap.Text = hd.MAPHIEUNHAP;
                dateNgayBan.Text = ((DateTime)hd.NGAY).ToString("dd/MM/yyyy");
                txtNhanVien.Text = db.NHANVIENs.Where(p => p.ID == hd.NHANVIENID).FirstOrDefault().TEN;
                txtTongChiPhi.Text = ((int)hd.TONGTIEN).ToString("N0");

                LoadCHITIETNHAP();
            }
            catch { }
        }

        #endregion

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                int ID = (int)dgvPhieuNhap.GetFocusedRowCellValue("ID");
                PHIEUNHAP hd = db.PHIEUNHAPs.Where(p => p.ID == ID).FirstOrDefault();
                FrmRpPhieuNhap form = new FrmRpPhieuNhap(hd);
                form.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Chưa có phiếu nhập nào được chọn",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadDgvDanhSachHoaDon();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Duyệt phiếu nhập này?","Duyệt",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    var dbEntry = db.PHIEUNHAPs.SingleOrDefault(x => x.MAPHIEUNHAP == txtMaPhieuNhap.Text);
                    dbEntry.STATUS = 1;
                    var Chitiet = db.CHITIETNHAPs.Where(x => x.PHIEUNHAPID == dbEntry.ID).ToList();
                    foreach(var item in Chitiet)
                    {
                        MATHANG mh = db.MATHANGs.Where(p => p.ID == item.MATHANGID).FirstOrDefault();
                        mh.SOLUONG += item.SOLUONG;
                    }                   
                    db.SaveChanges();
                    MessageBox.Show("Duyệt thành công !", "Duyệt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDgvDanhSachHoaDon();
                    btn_Xoa.Enabled = false;
                    btn_Duyet.Enabled = false;
                }
                catch
                {
                    MessageBox.Show("Có lỗi xảy ra ! Vui lòng thử lại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }

        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa phiếu nhập này?", "Duyệt", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    var dbEntry = db.PHIEUNHAPs.SingleOrDefault(x => x.MAPHIEUNHAP == txtMaPhieuNhap.Text);
                    var Chitiet = db.CHITIETNHAPs.Where(x => x.PHIEUNHAPID == dbEntry.ID).ToList();
                    foreach(var item in Chitiet)
                    {
                        db.CHITIETNHAPs.Remove(item);
                    }
                    db.PHIEUNHAPs.Remove(dbEntry);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thành công !", "Xóa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btn_Xoa.Enabled = false;
                    btn_Duyet.Enabled = false;
                    LoadDgvDanhSachHoaDon();
                }
                catch
                {
                    MessageBox.Show("Có lỗi xảy ra ! Vui lòng thử lại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }
    }
}
