using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using BookShop.Model;

namespace BookShop.GUI
{
    public partial class FrmThue : DevExpress.XtraEditors.XtraForm
    {
        VergetableContext db = null;
        public FrmThue()
        {
            db = new VergetableContext();
            InitializeComponent();
            LoadData();
        }
        void LoadData()
        {
            int i = 0;
            dgcThue.DataSource = db.THUEs.ToList()
                .Select(p => new
                {                   
                    STT = ++i,
                    Ten = p.Ten,
                    ThueSuat = p.ThueXuat,
                    ID = p.ID
                });
        }
        private void dgvThue_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            txtTen.Text = dgvThue.GetFocusedRowCellValue("Ten").ToString();
            txtThue.Text = dgvThue.GetFocusedRowCellValue("ThueSuat").ToString();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if(btnThem.Text == "Thêm")
            {
                btnThem.Text = "Lưu";
                btnSua.Enabled = false;
                btnXoa.Text = "Hủy";
                dgcThue.Enabled = false;
                txtTen.ReadOnly = false;
                txtThue.ReadOnly = false;
                txtTen.Text = "";
                txtThue.Text = "";
                return;
            }
            if(btnThem.Text == "Lưu")
            {
                THUE item = new THUE();
                try
                {
                    item.Ten = txtTen.Text;
                    item.ThueXuat = Convert.ToInt16(txtThue.Text);
                    db.THUEs.Add(item);
                    db.SaveChanges();
                    MessageBox.Show("Thêm thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnThem.Text = "Thêm";
                    btnSua.Enabled = true;
                    btnXoa.Text = "Xóa";
                    dgcThue.Enabled = true;
                    txtTen.Enabled = true;
                    txtThue.Enabled = true;
                    LoadData();
                }
                catch
                {
                    MessageBox.Show("Có lỗi xảy ra ! Xem lại các giá trị", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if(btnSua.Text == "Sửa")
            {
                btnSua.Text = "Lưu";
                btnThem.Enabled = false;
                btnXoa.Text = "Hủy";
                dgcThue.Enabled = false;
                txtTen.ReadOnly = false;
                txtThue.ReadOnly = false;
                return;
            }
            if (btnSua.Text == "Lưu")
            {
                var id = Convert.ToInt16(dgvThue.GetFocusedRowCellValue("ID").ToString());
                THUE item = db.THUEs.SingleOrDefault(x => x.ID == id);
                try
                {
                    item.Ten = txtTen.Text;
                    item.ThueXuat = Convert.ToInt16(txtThue.Text);
                    db.SaveChanges();
                    MessageBox.Show("Sửa thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSua.Text = "Sửa";
                    btnThem.Enabled = true;
                    btnXoa.Text = "Xóa";
                    dgcThue.Enabled = true;
                    txtTen.Enabled = true;
                    txtThue.Enabled = true;
                    LoadData();
                }
                catch
                {
                    MessageBox.Show("Có lỗi xảy ra ! Xem lại các giá trị", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if(btnXoa.Text == "Xóa")
            {
                if(MessageBox.Show("Bạn muốn xóa?","Xóa",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    var id = Convert.ToInt16(dgvThue.GetFocusedRowCellValue("ID").ToString());
                    var dbEntry = db.THUEs.SingleOrDefault(x => x.ID == id);
                    db.THUEs.Remove(dbEntry);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
            }
            if(btnXoa.Text == "Hủy")
            {
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnThem.Text = "Thêm";
                btnSua.Text = "Sửa";
                btnXoa.Text = "Xóa";
                dgcThue.Enabled = true;
                txtTen.ReadOnly = true;
                txtThue.ReadOnly = true;
                txtTen.Text = "";
                txtThue.Text = "";
            }
        }
    }
}