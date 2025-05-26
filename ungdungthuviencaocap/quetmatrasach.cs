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
using AForge.Video.DirectShow; // Đảm bảo using này tồn tại
using AForge.Video; // Đảm bảo using này tồn tại
using System.Media;
using System.Reflection;
using System.Threading;
using ZXing; // Đảm bảo using này tồn tại
using System.Data.SQLite; // Đảm bảo using này tồn tại

namespace ungdungthuviencaocap
{
	public partial class quetmatrasach : DevExpress.XtraEditors.XtraForm
	{
		FilterInfoCollection filterInfoCollection;
		VideoCaptureDevice videoCaptureDevice;
		CancellationTokenSource cancellationToken;
		Modify modify;
		private string отсканированныйMaSinhVienHienTai = "";
		private string hoVaTenHienTai = "";

		public quetmatrasach()
		{
			InitializeComponent();
			this.FormClosing += Form_Closing;
			label_hienketqua.Text = "";
			modify = new Modify();
			// Giả sử button trả sách đã chọn có tên là button_TraSachDaChon
			// và bạn đã thêm nó vào designer.
			// Ví dụ: this.button_TraSachDaChon.Click += new System.EventHandler(this.button_TraSachDaChon_Click);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (button1.Text == "Bắt đầu")
				{
					if (comboBox_camera.SelectedIndex < 0)
					{
						MessageBox.Show("Vui lòng chọn camera trước khi bắt đầu!", "Thông báo",
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					dataGridView_danhsach.DataSource = null;
					dataGridView_danhsach.Rows.Clear();
					отсканированныйMaSinhVienHienTai = "";
					hoVaTenHienTai = "";
					label_hienketqua.Text = "Đang chờ quét mã...";

					videoCaptureDevice = new VideoCaptureDevice(
						filterInfoCollection[comboBox_camera.SelectedIndex].MonikerString);
					videoCaptureDevice.NewFrame += FinalFrame_NewFrame;
					videoCaptureDevice.Start();

					cancellationToken = new CancellationTokenSource();
					var sourcetoken = cancellationToken.Token;
					onStartScan(sourcetoken);

					button1.Text = "Dừng lại!";
				}
				else
				{
					button1.Text = "Bắt đầu";
					try
					{
						if (cancellationToken != null && !cancellationToken.IsCancellationRequested)
						{
							cancellationToken.Cancel();
						}
						if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
						{
							videoCaptureDevice.SignalToStop();
							videoCaptureDevice.WaitForStop();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Lỗi nhỏ khi dừng camera: {ex.Message}");
					}
					finally
					{
						if (videoCaptureDevice != null)
						{
							videoCaptureDevice.NewFrame -= FinalFrame_NewFrame;
						}
					}
					label_hienketqua.Text = "Đã dừng quét.";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				button1.Text = "Bắt đầu";
			}
		}

		private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			try
			{
				if (!this.IsDisposed && pictureBox1 != null && !pictureBox1.IsDisposed)
				{
					Bitmap clonedFrame = (Bitmap)eventArgs.Frame.Clone();
					pictureBox1.Image = clonedFrame;
				}
			}
			catch (ObjectDisposedException)
			{
				// Bỏ qua
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi trong FinalFrame_NewFrame: {ex.Message}");
			}
		}

		public void onStartScan(CancellationToken sourcetoken)
		{
			Task.Factory.StartNew(new Action(() =>
			{
				while (true)
				{
					if (sourcetoken.IsCancellationRequested)
					{
						return;
					}

					Thread.Sleep(100);
					BarcodeReader Reader = new BarcodeReader();
					try
					{
						if (pictureBox1.Image != null && videoCaptureDevice != null && videoCaptureDevice.IsRunning)
						{
							Bitmap currentFrame = null;
							pictureBox1.Invoke(new Action(() =>
							{
								if (pictureBox1.Image != null && !pictureBox1.IsDisposed)
								{
									currentFrame = (Bitmap)pictureBox1.Image.Clone();
								}
							}));

							if (currentFrame != null)
							{
								var results = Reader.DecodeMultiple(currentFrame);
								currentFrame.Dispose();

								if (results != null)
								{
									foreach (Result result in results)
									{
										this.Invoke(new Action(() => {
											button1.Text = "Bắt đầu";
											if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
											{
												videoCaptureDevice.SignalToStop();
												videoCaptureDevice.WaitForStop();
												videoCaptureDevice.NewFrame -= FinalFrame_NewFrame;
											}
											if (cancellationToken != null && !cancellationToken.IsCancellationRequested)
											{
												cancellationToken.Cancel();
											}
										}));

										string maQuet = result.ToString();
										this.Invoke(new Action(() => label_hienketqua.Text = $"Đã quét: {maQuet}"));
										SystemSounds.Beep.Play();
										ProcessScannedCode(maQuet);
										return;
									}
								}
							}
						}
					}
					catch (ObjectDisposedException) { return; }
					catch (Exception ex)
					{
						Console.WriteLine("Loi trong luc quet: " + ex.Message);
					}
				}
			}), sourcetoken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void ProcessScannedCode(string maSinhVienQuet)
		{
			отсканированныйMaSinhVienHienTai = maSinhVienQuet;

			try
			{
				DataTable dtSachMuon = new DataTable();
				using (SQLiteConnection con = Connection.GetSQLiteConnection())
				{
					con.Open();
					string queryHoTen = "SELECT HoVaTen FROM taikhoan WHERE MaSinhVien = @MaSinhVien UNION SELECT HoVaTen FROM muontrasach WHERE MaSinhVien = @MaSinhVien AND IsActive = 1 LIMIT 1";
					using (SQLiteCommand cmdHoTen = new SQLiteCommand(queryHoTen, con))
					{
						cmdHoTen.Parameters.AddWithValue("@MaSinhVien", maSinhVienQuet);
						object tenResult = cmdHoTen.ExecuteScalar();
						if (tenResult != null)
						{
							hoVaTenHienTai = tenResult.ToString();
						}
						else
						{
							hoVaTenHienTai = "Không rõ";
						}
					}

					string query = @"SELECT ID, MaSach, TenSach, SoLuong, NgayMuon, NgayTra 
                                     FROM muontrasach 
                                     WHERE MaSinhVien = @MaSinhVien AND IsActive = 1";
					SQLiteCommand command = new SQLiteCommand(query, con);
					command.Parameters.AddWithValue("@MaSinhVien", maSinhVienQuet);

					SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
					adapter.Fill(dtSachMuon);
				}

				this.Invoke(new Action(() => {
					if (dtSachMuon.Rows.Count > 0)
					{
						label_hienketqua.Text = $"Sách đang mượn của: {hoVaTenHienTai} (Mã: {maSinhVienQuet})";
						dataGridView_danhsach.DataSource = dtSachMuon;
						if (dataGridView_danhsach.Columns.Contains("ID")) dataGridView_danhsach.Columns["ID"].HeaderText = "ID Phiếu";
						if (dataGridView_danhsach.Columns.Contains("MaSach")) dataGridView_danhsach.Columns["MaSach"].HeaderText = "Mã Sách";
						if (dataGridView_danhsach.Columns.Contains("TenSach")) dataGridView_danhsach.Columns["TenSach"].HeaderText = "Tên Sách";
						if (dataGridView_danhsach.Columns.Contains("SoLuong")) dataGridView_danhsach.Columns["SoLuong"].HeaderText = "SL";
						if (dataGridView_danhsach.Columns.Contains("NgayMuon")) dataGridView_danhsach.Columns["NgayMuon"].HeaderText = "Ngày Mượn";
						if (dataGridView_danhsach.Columns.Contains("NgayTra")) dataGridView_danhsach.Columns["NgayTra"].HeaderText = "Ngày Trả Dự Kiến";
						// button_TraSachDaChon.Enabled = true; 
					}
					else
					{
						label_hienketqua.Text = $"Sinh viên/Giảng viên {hoVaTenHienTai} (Mã: {maSinhVienQuet}) không có sách nào đang mượn.";
						dataGridView_danhsach.DataSource = null;
						// button_TraSachDaChon.Enabled = false; 
					}
				}));
			}
			catch (Exception ex)
			{
				this.Invoke(new Action(() => {
					MessageBox.Show($"Lỗi khi tải danh sách sách mượn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					label_hienketqua.Text = "Lỗi khi tải dữ liệu.";
					// button_TraSachDaChon.Enabled = false;
				}));
			}
		}

		private void button_TraSachDaChon_Click(object sender, EventArgs e)
		{
			if (dataGridView_danhsach.SelectedRows.Count == 0)
			{
				MessageBox.Show("Vui lòng chọn một sách từ danh sách để trả.", "Chưa chọn sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (string.IsNullOrEmpty(отсканированныйMaSinhVienHienTai))
			{
				MessageBox.Show("Không có thông tin người mượn. Vui lòng quét lại mã.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			DataGridViewRow selectedRow = dataGridView_danhsach.SelectedRows[0];
			int idPhieuMuon = Convert.ToInt32(selectedRow.Cells["ID"].Value);
			string maSachTra = selectedRow.Cells["MaSach"].Value.ToString();
			int soLuongTra = Convert.ToInt32(selectedRow.Cells["SoLuong"].Value);
			string tenSachTra = selectedRow.Cells["TenSach"].Value.ToString();

			try
			{
				bool successUpdateQuantity;
				using (SQLiteConnection con = Connection.GetSQLiteConnection())
				{
					con.Open();
					string updateBookQuery = "UPDATE quanlysach SET SoLuong = SoLuong + @SoLuongTra WHERE MaSach = @MaSach";
					SQLiteCommand updateBookCommand = new SQLiteCommand(updateBookQuery, con);
					updateBookCommand.Parameters.AddWithValue("@SoLuongTra", soLuongTra);
					updateBookCommand.Parameters.AddWithValue("@MaSach", maSachTra);
					successUpdateQuantity = updateBookCommand.ExecuteNonQuery() > 0;
				}

				if (!successUpdateQuantity)
				{
					MessageBox.Show($"Không thể cập nhật số lượng cho sách có mã: {maSachTra}. Sách có thể không tồn tại trong kho hoặc mã sách sai.", "Lỗi Cập Nhật Số Lượng", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool successTraSach = modify.trasach(idPhieuMuon);

				if (successTraSach)
				{
					modify.CapNhatThongKeTra(отсканированныйMaSinhVienHienTai, hoVaTenHienTai, soLuongTra, DateTime.Now);
					MessageBox.Show($"Đã trả sách '{tenSachTra}' thành công!", "Trả sách thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Làm mới lại danh sách sách đang mượn cho sinh viên hiện tại trên form này
					ProcessScannedCode(отсканированныйMaSinhVienHienTai);

					// Gọi phương thức làm mới trên form cha (muontrasach)
					if (this.Owner != null && this.Owner is muontrasach parentForm)
					{
						parentForm.RefreshDataGridView(); // Gọi phương thức public đã tạo ở muontrasach.cs
					}
				}
				else
				{
					using (SQLiteConnection con = Connection.GetSQLiteConnection())
					{
						con.Open();
						string restoreQuery = "UPDATE quanlysach SET SoLuong = SoLuong - @SoLuongTra WHERE MaSach = @MaSach";
						SQLiteCommand restoreCommand = new SQLiteCommand(restoreQuery, con);
						restoreCommand.Parameters.AddWithValue("@SoLuongTra", soLuongTra);
						restoreCommand.Parameters.AddWithValue("@MaSach", maSachTra);
						restoreCommand.ExecuteNonQuery();
					}
					MessageBox.Show("Trả sách thất bại. Vui lòng thử lại.", "Lỗi trả sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi trong quá trình trả sách: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Form_Closing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (cancellationToken != null && !cancellationToken.IsCancellationRequested)
				{
					cancellationToken.Cancel();
				}

				if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
				{
					videoCaptureDevice.NewFrame -= FinalFrame_NewFrame;
					videoCaptureDevice.SignalToStop();
					videoCaptureDevice.WaitForStop();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi đóng form và dừng camera: {ex.Message}");
			}
			finally
			{
				videoCaptureDevice = null;
			}
		}

		private void quetmatrasach_Load(object sender, EventArgs e)
		{
			try
			{
				comboBox_camera.Items.Clear();
				filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

				foreach (FilterInfo Device in filterInfoCollection)
					comboBox_camera.Items.Add(Device.Name);

				if (comboBox_camera.Items.Count > 0)
				{
					comboBox_camera.SelectedIndex = 0;
				}
				else
				{
					MessageBox.Show("Không tìm thấy thiết bị camera nào! Vui lòng kết nối camera và khởi động lại ứng dụng.",
						"Không tìm thấy camera", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					button1.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Đã xảy ra lỗi khi khởi tạo camera: {ex.Message}",
					"Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
