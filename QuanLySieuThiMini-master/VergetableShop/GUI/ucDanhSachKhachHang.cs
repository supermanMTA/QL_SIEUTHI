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

namespace BookShop.GUI
{
    public partial class ucDanhSachKhachHang : UserControl
    {
        private VergetableContext db = Helper.db;
        private int index = 0, index1 = 0;

        #region constructor
        public ucDanhSachKhachHang()
        {
            InitializeComponent();
            Helper.Reload();
        }

        #endregion

        #region Hàm chức năng

        private KHACHHANG getKhachHangByID()
        {
            try
            {
                int id = (int)dgvKhachHang.GetFocusedRowCellValue("ID");
                KHACHHANG ans = db.KHACHHANGs.Where(p => p.ID == id).FirstOrDefault();
                gridControl1.DataSource = db.HOADONs.Where(x => x.ID_KHACHHANG == id).ToList()
                                            .Select(p => new
                                            {
                                                ID = p.ID,
                                                NGAY = p.NGAY,
                                                TONGTIEN = ((int)p.TONGTIEN).ToString("N0")
                                            }).ToList();
                if (ans == null) return new KHACHHANG();
                return ans;
            }
            catch
            {
                return new KHACHHANG();
            }
        }

        private KHACHHANG getKhachHangByForm()
        {
            KHACHHANG ans = new KHACHHANG();

            try
            {
                ans.TEN = txtHoTen.Text;
                ans.GIOITINH = cbxGioiTinh.SelectedIndex;
                ans.NGAYSINH = dateNgaySinh.DateTime;
                ans.SDT = txtSDT.Text;
            }
            catch { }

            return ans;
        }

        private void ClearControl()
        {
            cbxGioiTinh.SelectedIndex = 0;
            dateNgaySinh.DateTime = DateTime.Now;

            txtHoTen.Text = "";
            txtSDT.Text = "";
        }

        private void UpdateDetail()
        {
            try
            {
                KHACHHANG KhachHang = getKhachHangByID();

                if (KhachHang.ID == 0) return;

                txtHoTen.Text = KhachHang.TEN;
                dateNgaySinh.DateTime = (DateTime)KhachHang.NGAYSINH;
                txtSDT.Text = KhachHang.SDT;
                textEdit1.Text = Convert.ToDecimal(db.HOADONs.Where(x => x.ID_KHACHHANG == KhachHang.ID).Sum(x => x.TONGTIEN)).ToString("N0");
                cbxGioiTinh.SelectedIndex = (int)KhachHang.GIOITINH;
            }
            catch
            {

            }
        }

        private void LockControl()
        {
            txtHoTen.Enabled = false;
            dateNgaySinh.Enabled = false;
            cbxGioiTinh.Enabled = false;
            txtSDT.Enabled = false;

            dgvKhachHangMain.Enabled = true;
            gridControl1.Enabled = true;

            txtTimKiem.Enabled = true;

            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
        }

        private void UnlockControl()
        {
            txtHoTen.Enabled = true;
            dateNgaySinh.Enabled = true;
            cbxGioiTinh.Enabled = true;
            txtSDT.Enabled = true;

            dgvKhachHangMain.Enabled = false;
            gridControl1.Enabled = false;
            txtTimKiem.Enabled = false;
        }

        private bool Check()
        {
           
            if (txtHoTen.Text == "")
            {
                MessageBox.Show("Họ tên của khách hàng không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
          return true;
        }

        private bool CheckLuaChon()
        {
            KHACHHANG tg = getKhachHangByID();
            if (tg.ID == 0)
            {
                MessageBox.Show("Chưa có khách nào được chọn",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void CapNhat(ref KHACHHANG cu, KHACHHANG moi)
        {
            cu.TEN = moi.TEN;
            cu.GIOITINH = moi.GIOITINH;
            cu.NGAYSINH = moi.NGAYSINH;
            cu.SDT = moi.SDT;
        }

        #endregion

        #region LoadForm

        private void LoadInitControl()
        {
            ClearControl();
        }

        private void LoadDgvKhachHang()
        {
            int i = 0;
            string keyWord = txtTimKiem.Text.Trim().ToUpper();
            var listNv = db.KHACHHANGs.ToList()
                           .Select(p => new
                           {
                               ID = p.ID,
                               HoTen = p.TEN
                           })
                                        .ToList();
            dgvKhachHangMain.DataSource = listNv.ToList()
                                         .Where(p => p.HoTen.ToUpper().Contains(keyWord))
                                         .Select(p => new
                                         {
                                             ID = p.ID,
                                             STT = ++i,
                                             HoTen = p.HoTen
                                         }).ToList();

            UpdateDetail();

            /// Load lại dòng đang chọn
            try
            {
                index = index1;
                dgvKhachHang.FocusedRowHandle = index;
                dgvKhachHangMain.Select();
            }
            catch
            {

            }

        }
        private void ucDanhSachKhachHang_Load(object sender, EventArgs e)
        {
            LoadInitControl();
            LoadDgvKhachHang();
            LockControl();
        }


        #endregion

        #region Sự kiện ngầm
        private void dgvKhachHang_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateDetail();

            try
            {
                index1 = index;
                index = dgvKhachHang.FocusedRowHandle;
            }
            catch { }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            LoadDgvKhachHang();
            txtTimKiem.Focus();
        }

        #endregion

        #region Sự kiện
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (btnThem.Text == "Thêm")
            {
                btnSua.Enabled = false;
                btnThem.Text = "Lưu";
                btnXoa.Text = "Hủy";

                ClearControl();
                UnlockControl();


                return;
            }

            if (btnThem.Text == "Lưu")
            {
                if (Check())
                {
                    btnThem.Text = "Thêm";
                    btnXoa.Text = "Xóa";
                    LockControl();


                    KHACHHANG moi = getKhachHangByForm();
                    db.KHACHHANGs.Add(moi);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Thêm thông tin khách hàng thành công",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm thông tin khách hàng thất bại\n" + ex.Message,
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    LoadDgvKhachHang();
                }
                return;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!CheckLuaChon()) return;

            if (btnSua.Text == "Sửa")
            {
                btnSua.Text = "Lưu";
                btnXoa.Text = "Hủy";
                btnThem.Enabled = false;

                UnlockControl();

                return;
            }

            if (btnSua.Text == "Lưu")
            {
                if (Check())
                {
                    btnSua.Text = "Sửa";
                    btnXoa.Text = "Xóa";

                    LockControl();

                    KHACHHANG cu = getKhachHangByID();
                    KHACHHANG moi = getKhachHangByForm();
                    CapNhat(ref cu, moi);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Sưa thông tin khách hàng thành công",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sửa thông tin khách hàng thất bại\n" + ex.Message,
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    LoadDgvKhachHang();
                }

                return;
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (btnXoa.Text == "Xóa")
            {
                if (!CheckLuaChon()) return;

                KHACHHANG cu = getKhachHangByID();
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn xóa nhân viên " + cu.TEN + "?",
                                                  "Thông báo",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning);

                if (rs == DialogResult.Cancel) return;

                try
                {
                    db.KHACHHANGs.Remove(cu);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thông tin nhân viên thành công",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xóa thông tin nhân viên thất bại\n" + ex.Message,
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                LoadDgvKhachHang();

                return;
            }
            if (btnXoa.Text == "Hủy")
            {
                btnSua.Text = "Sửa";
                btnThem.Text = "Thêm";
                btnXoa.Text = "Xóa";

                LockControl();
                UpdateDetail();
                return;
            }
        }

       

        #endregion


    }
}
