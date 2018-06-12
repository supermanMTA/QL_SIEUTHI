using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using BookShop.Model;

namespace BookShop.GUI
{
    public partial class FrmThemThue : MetroForm
    {
        VergetableContext db = null;
        private int idmh = 0;
        public FrmThemThue(int id_mathang)
        {
            db = new VergetableContext();
            InitializeComponent();
            comboBox1.DataSource = db.THUEs.ToList();
            comboBox1.ValueMember = "ID";
            comboBox1.DisplayMember = "Ten";
            idmh = id_mathang;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            THUE_MATHANG item = new THUE_MATHANG();
            item.ID_THUE = Convert.ToInt16(comboBox1.SelectedValue.ToString());
            item.ID_MATHANG = idmh;
            var dbEntry = db.THUE_MATHANG.SingleOrDefault(x => x.ID_MATHANG == item.ID_MATHANG && x.ID_THUE ==item.ID_THUE);
            if (dbEntry != null)
            {
                MessageBox.Show("Mặt hàng này đã áp dụng loại thuế này", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                db.THUE_MATHANG.Add(item);
                db.SaveChanges();
                MessageBox.Show("Áp dụng thuế thành công", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Có lỗi xảy ra", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmThemThue_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
