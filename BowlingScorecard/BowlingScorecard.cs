using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BowlingScorecard {
    public partial class frmBowlingScorecard : Form {
        public frmBowlingScorecard() {
            InitializeComponent();
        }

        private void frmBowlingScorecard_Load(object sender, EventArgs e) {
            pnlPlayer1.Controls.Add(new PlayerTenFrames());
            pnlPlayer2.Controls.Add(new PlayerTenFrames());
        }
    }
}
