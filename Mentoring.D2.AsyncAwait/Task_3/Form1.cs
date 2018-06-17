using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_3
{
    public partial class Form1 : Form
    {
        private BindingList<Food> _cartItems = new BindingList<Food>();
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var PriceListItems = new BindingList<Food>
            {
                new Food
                {
                    Id = 0,
                    Name = "Meat",
                    Price = 100
                },
                new Food
                {
                    Id = 1,
                    Name = "Rice",
                    Price = 340
                },
                new Food
                {
                    Id = 2,
                    Name = "Sugar",
                    Price = 1200
                },
                new Food
                {
                    Id = 3,
                    Name = "Potato",
                    Price = 10220
                },
                new Food
                {
                    Id = 4,
                    Name = "Salt",
                    Price = 1010
                },
            };
            dgvPriceList.DataSource = bindingSource1;
            bindingSource1.DataSource = PriceListItems;
            dgvCart.DataSource = bindingSource2;
            bindingSource2.DataSource = _cartItems;
            var AddToCartButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Add To Cart",
                UseColumnTextForButtonValue = true
            };
            var DeleteFromCartButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Delete From Cart",
                UseColumnTextForButtonValue = true
            };
            dgvPriceList.Columns.Add(AddToCartButtonColumn);
            dgvCart.Columns.Add(DeleteFromCartButtonColumn);
        }

        private async void dgvPriceList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPriceList.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var item = (Food)dgvPriceList.Rows[e.RowIndex].DataBoundItem;
                _cartItems.Add(item);
                label4.Text = await GetTotal(_cartItems);
            }
        }

        private async void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCart.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                var item = (Food)dgvCart.Rows[e.RowIndex].DataBoundItem;
                _cartItems.Remove(item);
                label4.Text = await GetTotal(_cartItems);
            }
        }

        private async Task<String> GetTotal(BindingList<Food> cart)
        {
            return await Task.Factory.StartNew(() => cart.Sum(i => i.Price).ToString());
        }
    }
}
