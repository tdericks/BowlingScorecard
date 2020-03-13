using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BowlingScorecard {
    public partial class PlayerTenFrames : UserControl {
        public PlayerTenFrames() {
            InitializeComponent();
            tblScorecard.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        private void FillTextBoxWithKeyPressedAndCalculateScore(TextBox txt, Keys keyCode) {
            int numberPressed = 0;

            string ball1LabelName = txt.Name.Replace("Ball2", "Ball1");
            TextBox txtBall1 = (TextBox)this.Controls.Find(ball1LabelName, true)[0];   // Should find one control with the name, so always zero index from array

            string ball2LabelName = txt.Name.Replace("Ball1", "Ball2");
            TextBox txtBall2 = (TextBox)this.Controls.Find(ball2LabelName, true)[0];   // Should find one control with the name, so always zero index from array

            bool isInTenthFrame = txt.Name.EndsWith("Frame10");

            int frameNumber = Convert.ToInt32(txt.Name.Replace("txtBall1Frame", "").Replace("txtBall2Frame", "").Replace("txtBall3Frame", "").Trim());
            string nextFrameBall1LabelName = string.Format("txtBall1Frame{0}", isInTenthFrame ? 10 : frameNumber + 1);
            TextBox txtNextFrameBall1 = (TextBox)this.Controls.Find(nextFrameBall1LabelName, true)[0];   // Should find one control with the name, so always zero index from array

            bool isFirstBall = txt.Name.StartsWith("txtBall1");
            bool isSecondBall = txt.Name.StartsWith("txtBall2");
            bool isSecondBallStrike = (txtBall2.Text == "X");
            bool validKeyPressed = false;
            bool isSpare = false;

            int firstBallPinsStanding = 10 - ((txtBall1.Text.Trim().Length == 0 || txtBall1.Text == "/")
                                               ? 0 : (txtBall1.Text == "-"
                                                       ? 0 : (isInTenthFrame && txtBall1.Text == "X"
                                                                ? 10 : Convert.ToInt32(txtBall1.Text)
                                                            )
                                                     )
                                             );

            // Only allow numbers 1 thru 9, a dash, an X and a /.
            switch (keyCode) {
                case Keys.X:
                    validKeyPressed = true;
                    if (isInTenthFrame) {
                        if (isSecondBall && txtBall1.Text != "X")
                            validKeyPressed = false;
                        else {
                            if (txt.Name == "txtBall3Frame10" && (txtBall2Frame10.Text != "X" && txtBall2Frame10.Text != "/"))
                                validKeyPressed = false;
                            else
                                txt.Text = "X";
                        }
                        if (isFirstBall) {
                            txtBall2.Text = "";
                            txtBall3Frame10.Text = "";
                            txtBall3Frame10.Enabled = false;
                        }
                    } else {
                        if (isFirstBall) {
                            txt.Text = "";
                            txtBall2.Enabled = true;
                            txtBall2.Text = "X";
                            txtBall2.Focus();
                            txtNextFrameBall1.Enabled = true;
                        } else {
                            if (isSecondBall) {
                                // Check to make sure the first ball is blank, if there is a value, can't have a strike
                                if (txtBall1.Text.Trim().Length > 0)
                                    validKeyPressed = false;
                                else
                                    txt.Text = "X";
                            }
                        }
                    }
                    break;
                case Keys.D0:
                case Keys.NumPad0:
                case Keys.Subtract:
                case Keys.OemMinus:
                    validKeyPressed = true;
                    txt.Text = "-";
                    break;
                case Keys.Divide:
                case Keys.OemQuestion:
                    // Only allow the slash to be used on Ball 2 and Ball 3 of the 10th frame
                    if (!isFirstBall) {
                        validKeyPressed = true;
                        if (txt.Name == "txtBall3Frame10" && (txtBall2Frame10.Text == "X" || txtBall2Frame10.Text == "/")) {
                            validKeyPressed = false;
                        }
                        if (validKeyPressed) {
                            txt.Text = "/";
                            if ((txtBall1.Text.Trim().Length <= 0) || (txtBall1.Text == "X" && isInTenthFrame))
                                txtBall1.Text = "-";
                        }
                    }
                    break;
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    validKeyPressed = true;
                    numberPressed = Convert.ToInt32(keyCode.ToString().ToUpper().Replace("NUMPAD", "").Replace("D", ""));
                    // only allow entries of a number less than the first ball
                    if (isSecondBall) {
                        if (isInTenthFrame && txtBall1Frame10.Text != "X") {
                            validKeyPressed = (numberPressed <= firstBallPinsStanding);
                            isSpare = (numberPressed == firstBallPinsStanding);
                        }
                    }
                    if (validKeyPressed) {
                        txt.Text = (isSpare ? "/" : numberPressed.ToString());
                        // Set the value of the strike to a spare if the user is changing the first ball value
                        if (isFirstBall && isSecondBallStrike)
                            txtBall2.Text = "/";
                    }
                    break;
            }
            if (isSecondBall) {
                if (isInTenthFrame) {
                    if (txtBall1Frame10.Text == "X" || (txt.Text == "/" || txt.Text == "X"))
                        txtBall3Frame10.Enabled = true;
                    else {
                        txtBall3Frame10.Text = "";
                        txtBall3Frame10.Enabled = false;
                    }
                }

                txtNextFrameBall1.Enabled = (!txtNextFrameBall1.Enabled ? validKeyPressed : true);
            } else
                txtBall2.Enabled = (!txtBall2.Enabled ? validKeyPressed : true);

            // Update the score to make sure they're correct after changes to the scorecard
            int runningScore = 0;
            for (int i = 1; i <= 10; i++) {
                isInTenthFrame = (i == 10);
                txtBall1 = (TextBox)this.Controls.Find(string.Format("txtBall1Frame{0}", i), true)[0];   // Should find one control with the name, so always zero index from array
                txtBall2 = (TextBox)this.Controls.Find(string.Format("txtBall2Frame{0}", i), true)[0];   // Should find one control with the name, so always zero index from array
                
                // break out of for loop if the current frame hasn't been filled
                if (txtBall2.Text.Trim().Length <= 0)
                    break;

                if (isInTenthFrame) {
                    if (txtBall1.Text == "X") {
                        runningScore += (10 + GetFrameScore(txtBall2, txtBall3Frame10));
                    } else {
                        runningScore += GetFrameScore(txtBall1, txtBall2);
                        if (txtBall2.Text == "/") {
                            runningScore += GetFrameBallScore(txtBall3Frame10);
                        }
                    }
                }

                if (txtBall2.Text == "X" && !isInTenthFrame) {
                    // Strike - Take the scores of the next two balls, across frames if another strike, but only if available
                    TextBox txtBall1NextFrame = (TextBox)this.Controls.Find(string.Format("txtBall1Frame{0}", i + 1), true)[0];   // Should find one control with the name, so always zero index from array
                    TextBox txtBall2NextFrame = (TextBox)this.Controls.Find(string.Format("txtBall2Frame{0}", i + 1), true)[0];   // Should find one control with the name, so always zero index from array
                    if (txtBall1NextFrame.Text.Trim().Length > 0) {
                        // Next frame's first ball wasn't a strike, so add 10 to it and the second ball if the second ball isn't a spare.
                        runningScore += (10 + GetFrameScore(txtBall1NextFrame, txtBall2NextFrame));
                    } else {
                        // Possible Double
                        if (txtBall2NextFrame.Text == "X" && i < 9) {
                            // Second ball was a strike, so look at the next two balls (2nd one for a strike as well)
                            TextBox txtBall1SecondFrame = (TextBox)this.Controls.Find(string.Format("txtBall1Frame{0}", i + 2), true)[0];   // Should find one control with the name, so always zero index from array
                            TextBox txtBall2SecondFrame = (TextBox)this.Controls.Find(string.Format("txtBall2Frame{0}", i + 2), true)[0];   // Should find one control with the name, so always zero index from array
                            if (txtBall1SecondFrame.Text.Trim().Length > 0) {
                                // Next frame's first ball wasn't a strike, so add it to 10 for the score
                                runningScore += (20 + GetFrameBallScore(txtBall1SecondFrame));
                            } else {
                                // Possible Turkey
                                if (txtBall2SecondFrame.Text == "X") {
                                    runningScore += 30;
                                } else
                                    break;
                            }
                        } else
                            break;
                    }
                } else {
                    if (txtBall2.Text == "/" && !isInTenthFrame) {
                        // Strike - Take the scores of the next two balls, across frames if another strike, but only if available
                        TextBox txtBall1NextFrame = (TextBox)this.Controls.Find(string.Format("txtBall1Frame{0}", i + 1), true)[0];   // Should find one control with the name, so always zero index from array
                        TextBox txtBall2NextFrame = (TextBox)this.Controls.Find(string.Format("txtBall2Frame{0}", i + 1), true)[0];   // Should find one control with the name, so always zero index from array
                        if (txtBall1NextFrame.Text.Trim().Length > 0) {
                            runningScore += (10 + GetFrameBallScore(txtBall1NextFrame));
                        } else {
                            // Possible Strike
                            if (txtBall2NextFrame.Text == "X") {
                                runningScore += 20;
                            } else
                                break;
                        }
                    } else {
                        // Not a strike or a spare, so just add the numbers
                        if (!isInTenthFrame) {
                            runningScore += GetFrameScore(txtBall1, txtBall2);
                        }
                    }
                }
                Label lblScoreFrame = (Label)this.Controls.Find(string.Format("lblScoreFrame{0}", i), true)[0];   // Should find one label with the name, so always zero index from array
                lblScoreFrame.Text = runningScore.ToString();
            }
        }

        private int GetFrameBallScore(TextBox ballInFrame) {
            return ((ballInFrame.Text == "-" || ballInFrame.Text.Trim().Length <= 0) ? 0 : (ballInFrame.Text == "X" ? 10 : Convert.ToInt32(ballInFrame.Text)));
        }

        private int GetFrameScore(TextBox firstBall, TextBox secondBall) {
            if (secondBall.Text == "/") {
                return 10;
            } else {
                return GetFrameBallScore(firstBall) + GetFrameBallScore(secondBall);
            }
        }

        private void txtBallFrame_KeyDown(object sender, KeyEventArgs e) {
            TextBox txtBox = (TextBox)sender;
            e.SuppressKeyPress = true;
            FillTextBoxWithKeyPressedAndCalculateScore(txtBox, e.KeyCode);
        }
    }
}
