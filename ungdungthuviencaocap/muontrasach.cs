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
using System.Data.SqlClient;
using System.Data.SQLite;
using DevExpress.XtraEditors.Filtering.Templates;

namespace ungdungthuviencaocap
{
	public partial class muontrasach : DevExpress.XtraEditors.XtraForm
	{
		public muontrasach()
		{
			InitializeComponent();
			// Gắn sự kiện SelectedIndexChanged cho comboBox_tensach
			comboBox_tensach.SelectedIndexChanged += comboBox_tensach_SelectedIndexChanged;
			// Gắn sự kiện TextChanged cho comboBox_tensach để xử lý khi người dùng nhập liệu
			comboBox_tensach.TextChanged += comboBox_tensach_TextChanged;

			// Thiết lập giá trị mặc định cho comboBox_nguoimuon
			comboBox_nguoimuon.SelectedItem = "Sinh viên"; // Hoặc "Giảng viên" tùy theo logic mặc định
		}
		Modify modify;
		private void LoadDataGridView()
		{
			try
			{
				// Câu lệnh SQL để lấy dữ liệu phiếu mượn đang hoạt động
				string query = "SELECT ID, TenSach, MaSach, MaSinhVien, HoVaTen, DoiTuongMuon, LoaiPhieu, NgayMuon, NgayTra, SoLuong, IsActive FROM muontrasach WHERE IsActive = 1 ORDER BY NgayMuon DESC";
				DataTable dt = new DataTable();

				using (SQLiteConnection con = Connection.GetSQLiteConnection())
				{
					con.Open();
					SQLiteCommand command = new SQLiteCommand(query, con);
					SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
					adapter.Fill(dt);
				}

				// Xóa các cột có thể đã được định nghĩa trong designer để tránh xung đột
				dataGridView1.Columns.Clear();
				// Đảm bảo cột được tự động tạo từ DataSource
				dataGridView1.AutoGenerateColumns = true;
				dataGridView1.DataSource = dt;

				// Cấu hình các cột sau khi DataSource đã được gán và cột đã được tự động tạo
				// Luôn kiểm tra sự tồn tại của cột trước khi truy cập
				if (dataGridView1.Columns.Contains("ID"))
				{
					dataGridView1.Columns["ID"].Visible = false;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'ID' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("IsActive"))
				{
					dataGridView1.Columns["IsActive"].Visible = false;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'IsActive' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("TenSach"))
				{
					dataGridView1.Columns["TenSach"].HeaderText = "Tên sách";
					dataGridView1.Columns["TenSach"].DisplayIndex = 2; // Sắp xếp thứ tự hiển thị
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'TenSach' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("MaSach"))
				{
					dataGridView1.Columns["MaSach"].HeaderText = "Mã Sách";
					dataGridView1.Columns["MaSach"].DisplayIndex = 1;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'MaSach' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("MaSinhVien"))
				{
					dataGridView1.Columns["MaSinhVien"].HeaderText = "Mã SV/GV";
					dataGridView1.Columns["MaSinhVien"].DisplayIndex = 3;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'MaSinhVien' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("HoVaTen"))
				{
					dataGridView1.Columns["HoVaTen"].HeaderText = "Họ và tên";
					dataGridView1.Columns["HoVaTen"].DisplayIndex = 4;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'HoVaTen' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("DoiTuongMuon"))
				{
					dataGridView1.Columns["DoiTuongMuon"].HeaderText = "Đối tượng";
					dataGridView1.Columns["DoiTuongMuon"].DisplayIndex = 5;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'DoiTuongMuon' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("LoaiPhieu"))
				{
					dataGridView1.Columns["LoaiPhieu"].HeaderText = "Loại phiếu";
					// dataGridView1.Columns["LoaiPhieu"].DisplayIndex = vị trí mong muốn;
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'LoaiPhieu' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("NgayMuon"))
				{
					dataGridView1.Columns["NgayMuon"].HeaderText = "Ngày mượn";
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'NgayMuon' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("NgayTra"))
				{
					dataGridView1.Columns["NgayTra"].HeaderText = "Ngày trả";
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'NgayTra' trong DataGridView."); }

				if (dataGridView1.Columns.Contains("SoLuong"))
				{
					dataGridView1.Columns["SoLuong"].HeaderText = "Số lượng";
				}
				else { Console.WriteLine("Lỗi LoadDataGridView: Không tìm thấy cột 'SoLuong' trong DataGridView."); }
			}
			catch (SQLiteException sqlEx) // Bắt lỗi cụ thể từ SQLite
			{
				MessageBox.Show($"Lỗi CSDL khi tải dữ liệu phiếu mượn: {sqlEx.Message}\nCâu lệnh SQL có thể liên quan đến lỗi (nếu có): {sqlEx.Source}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex) // Bắt các lỗi chung khác
			{
				MessageBox.Show($"Lỗi không xác định khi tải dữ liệu phiếu mượn: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void muontrasach_Load(object sender, EventArgs e)
		{
			modify = new Modify();
			LoadDataGridView();

			DataTable dtSach = new DataTable();
			using (SQLiteConnection con = Connection.GetSQLiteConnection())
			{
				try
				{
					con.Open();
					SQLiteCommand command = new SQLiteCommand("SELECT TenSach, MaSach FROM quanlysach ORDER BY TenSach ASC", con);
					SQLiteDataReader readSQL = command.ExecuteReader();

					if (readSQL.HasRows)
					{
						dtSach.Load(readSQL);
						Console.WriteLine("--- Dữ liệu được tải vào dtSach cho ComboBox Sách ---");
						foreach (DataRow dr in dtSach.Rows)
						{
							string ten = dr.Table.Columns.Contains("TenSach") ? dr["TenSach"].ToString() : "KHÔNG TÌM THẤY CỘT TênSách";
							string ma = dr.Table.Columns.Contains("MaSach") ? dr["MaSach"].ToString() : "KHÔNG TÌM THẤY CỘT MãSách";
							Console.WriteLine($"TenSach: [{ten}], MaSach: [{ma}]");
							if (ten.Equals("Lịch sử 11", StringComparison.OrdinalIgnoreCase)) // So sánh không phân biệt hoa thường
							{
								Console.WriteLine($"DEBUG comboBox_tensach_Load: Sách 'Lịch sử 11' được tải với MaSach = '{ma}'");
							}
						}
						Console.WriteLine("--- Kết thúc dữ liệu dtSach ---");


						if (dtSach.Columns.Contains("TenSach") && dtSach.Columns.Contains("MaSach"))
						{
							comboBox_tensach.DataSource = dtSach;
							comboBox_tensach.DisplayMember = "TenSach";
							comboBox_tensach.ValueMember = "MaSach";
						}
						else
						{
							string missingColumns = "";
							if (!dtSach.Columns.Contains("TenSach")) missingColumns += "'TenSach' ";
							if (!dtSach.Columns.Contains("MaSach")) missingColumns += "'MaSach' ";
							MessageBox.Show($"Lỗi cấu hình ComboBox Sách: Không tìm thấy cột {missingColumns.Trim()} trong dữ liệu được tải.", "Lỗi Dữ Liệu Sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
							comboBox_tensach.DataSource = null;
						}
					}
					else
					{
						Console.WriteLine("Không có dữ liệu sách nào được tìm thấy trong CSDL để tải vào ComboBox.");
						comboBox_tensach.DataSource = null;
						comboBox_tensach.Items.Clear();
					}
				}
				catch (ArgumentNullException anex)
				{
					MessageBox.Show($"Lỗi khi tải danh sách sách (ArgumentNull): {anex.Message}\nTên tham số: {anex.ParamName}\nStackTrace: {anex.StackTrace}", "Lỗi Tham Số Null", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Lỗi khi tải danh sách sách: {ex.Message}\nStackTrace: {ex.StackTrace}", "Lỗi Chung", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					if (con.State == ConnectionState.Open)
					{
						con.Close();
					}
				}
			}
			comboBox_tensach.SelectedIndex = -1;
			textBox_masach.Text = "";
		}

		// Sự kiện khi người dùng chọn một sách từ ComboBox
		private void comboBox_tensach_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox_tensach.SelectedValue != null)
			{
				string selectedValueStr = comboBox_tensach.SelectedValue.ToString();
				Console.WriteLine($"DEBUG comboBox_tensach_SelectedIndexChanged: SelectedItem='{comboBox_tensach.Text}', SelectedValue (MaSach)=[{selectedValueStr}], Type={comboBox_tensach.SelectedValue.GetType()}");
				textBox_masach.Text = selectedValueStr;
			}
			else if (!string.IsNullOrWhiteSpace(comboBox_tensach.Text) && modify != null)
			{
				// Trường hợp người dùng gõ text vào ComboBox (không chọn từ danh sách thả xuống)
				string tenSachNhap = comboBox_tensach.Text;
				Console.WriteLine($"DEBUG comboBox_tensach_SelectedIndexChanged: SelectedValue is null, Text is '{tenSachNhap}'. Attempting to find MaSach via GetMaSachByTenSach.");
				string maSachTimDuoc = modify.GetMaSachByTenSach(tenSachNhap);
				if (!string.IsNullOrEmpty(maSachTimDuoc))
				{
					textBox_masach.Text = maSachTimDuoc;
					Console.WriteLine($"DEBUG comboBox_tensach_SelectedIndexChanged: Found MaSach='{maSachTimDuoc}' for TenSach='{tenSachNhap}' via GetMaSachByTenSach.");
				}
				else
				{
					textBox_masach.Text = "";
					Console.WriteLine($"DEBUG comboBox_tensach_SelectedIndexChanged: Could not find MaSach for TenSach='{tenSachNhap}' via GetMaSachByTenSach.");
				}
			}
			else
			{
				textBox_masach.Text = "";
				Console.WriteLine($"DEBUG comboBox_tensach_SelectedIndexChanged: SelectedValue is null and Text is whitespace or modify is null. textBox_masach cleared.");
			}
		}

		private void comboBox_tensach_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(comboBox_tensach.Text))
			{
				textBox_masach.Text = "";
				Console.WriteLine($"DEBUG comboBox_tensach_TextChanged: Text is whitespace. textBox_masach cleared.");
			}
		}


		private void button_phieumuon_Click(object sender, EventArgs e)
		{
			try
			{
				string Tensach = this.comboBox_tensach.Text;
				string masinhvien = this.textBox_masinhvien.Text.Trim();
				string Hovaten = this.textBox_hovaten.Text.Trim();
				string Loaiphieu = this.comboBox_LoaiPhieu.Text;
				DateTime ngaymuon = dateTimePicker_muonsach.Value;
				DateTime ngaytra = dateTimePicker_trasach.Value;
				string soluongStr = this.textBox_soluong.Text.Trim();
				string doiTuongMuon = this.comboBox_nguoimuon.SelectedItem?.ToString();

				string masach = this.textBox_masach.Text.Trim();
				Console.WriteLine($"DEBUG button_phieumuon_Click: Attempting to borrow. TenSach='{Tensach}', MaSach='{masach}', MaNguoiMuon='{masinhvien}', DoiTuong='{doiTuongMuon}', SoLuong='{soluongStr}'");


				if (string.IsNullOrEmpty(Tensach))
				{
					MessageBox.Show("Vui lòng chọn tên sách!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBox_tensach.Focus();
					return;
				}
				if (string.IsNullOrEmpty(masach))
				{
					MessageBox.Show("Mã sách không được để trống! Hãy chọn lại tên sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBox_tensach.Focus();
					return;
				}
				if (string.IsNullOrEmpty(masinhvien))
				{
					MessageBox.Show("Mã sinh viên/giảng viên không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_masinhvien.Focus();
					return;
				}
				if (string.IsNullOrEmpty(Hovaten))
				{
					MessageBox.Show("Họ và tên không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_hovaten.Focus();
					return;
				}
				if (string.IsNullOrEmpty(doiTuongMuon))
				{
					MessageBox.Show("Vui lòng chọn đối tượng mượn (Sinh viên/Giảng viên)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBox_nguoimuon.Focus();
					return;
				}
				if (ngaytra < ngaymuon)
				{
					MessageBox.Show("Ngày trả không thể trước ngày mượn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					dateTimePicker_trasach.Focus();
					return;
				}

				int soLuongMuonInt;
				if (!int.TryParse(soluongStr, out soLuongMuonInt) || soLuongMuonInt <= 0)
				{
					MessageBox.Show("Số lượng sách mượn phải là số nguyên dương!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_soluong.Focus();
					return;
				}

				int soLuongSachDaMuon = modify.GetSoLuongSachDangMuon(masinhvien, doiTuongMuon);
				int gioiHanMuon = (doiTuongMuon == "Sinh viên") ? 2 : 4;
				Console.WriteLine($"DEBUG button_phieumuon_Click: SoLuongSachDaMuon='{soLuongSachDaMuon}', GioiHanMuon='{gioiHanMuon}', SoLuongMuonInt='{soLuongMuonInt}'");


				if (soLuongSachDaMuon + soLuongMuonInt > gioiHanMuon)
				{
					MessageBox.Show($"Bạn đã mượn {soLuongSachDaMuon} quyển sách. {doiTuongMuon} chỉ được mượn tối đa {gioiHanMuon} quyển. " +
									$"Bạn chỉ có thể mượn thêm tối đa {Math.Max(0, gioiHanMuon - soLuongSachDaMuon)} quyển nữa.",
									"Lỗi Giới Hạn Mượn Sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				using (SQLiteConnection sqlConnection = Connection.GetSQLiteConnection())
				{
					sqlConnection.Open();
					string checkQuery = "SELECT SoLuong FROM quanlysach WHERE MaSach = @MaSach";
					SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, sqlConnection);
					checkCommand.Parameters.AddWithValue("@MaSach", masach);

					object result = checkCommand.ExecuteScalar();
					if (result == null)
					{
						MessageBox.Show("Không tìm thấy sách này trong hệ thống (dựa trên mã sách)! ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					int availableQuantity = Convert.ToInt32(result);
					Console.WriteLine($"DEBUG button_phieumuon_Click: Available quantity for MaSach='{masach}' is {availableQuantity}");

					if (soLuongMuonInt > availableQuantity)
					{
						MessageBox.Show($"Sách '{Tensach}' không đủ số lượng (còn {availableQuantity}, cần {soLuongMuonInt}). Hãy đợi thư viện bổ sung hoặc người khác trả sách nhé! ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					string updateQuery = "UPDATE quanlysach SET SoLuong = SoLuong - @SoLuongMuon WHERE MaSach = @MaSach";
					SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, sqlConnection);
					updateCommand.Parameters.AddWithValue("@SoLuongMuon", soLuongMuonInt);
					updateCommand.Parameters.AddWithValue("@MaSach", masach);
					updateCommand.ExecuteNonQuery();
					Console.WriteLine($"DEBUG button_phieumuon_Click: Updated quanlysach, reduced SoLuong by {soLuongMuonInt} for MaSach='{masach}'");
				}

				muontra muontra = new muontra(0, Tensach, masinhvien, Hovaten, Loaiphieu, ngaymuon, ngaytra, soluongStr, masach, doiTuongMuon, 1);

				if (modify.themphieu(muontra))
				{
					modify.CapNhatThongKeMuon(masinhvien, Hovaten, soLuongMuonInt, ngaymuon);
					MessageBox.Show("Thêm phiếu mượn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					Console.WriteLine($"DEBUG button_phieumuon_Click: Successfully added borrow record. MaSach inserted into muontrasach: '{masach}'");
					LoadDataGridView();
					ClearInputFields();
				}
				else
				{
					Console.WriteLine($"DEBUG button_phieumuon_Click: Failed to add borrow record. Restoring quantity for MaSach='{masach}'");
					using (SQLiteConnection sqlConnection = Connection.GetSQLiteConnection())
					{
						sqlConnection.Open();
						string restoreQuery = "UPDATE quanlysach SET SoLuong = SoLuong + @SoLuongMuon WHERE MaSach = @MaSach";
						SQLiteCommand restoreCommand = new SQLiteCommand(restoreQuery, sqlConnection);
						restoreCommand.Parameters.AddWithValue("@SoLuongMuon", soLuongMuonInt);
						restoreCommand.Parameters.AddWithValue("@MaSach", masach);
						restoreCommand.ExecuteNonQuery();
					}
					MessageBox.Show("Thêm phiếu mượn thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR button_phieumuon_Click: {ex.ToString()}");
				MessageBox.Show("Lỗi khi tạo phiếu mượn: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ClearInputFields()
		{
			comboBox_tensach.SelectedIndex = -1;
			textBox_masinhvien.Text = "";
			textBox_hovaten.Text = "";
			comboBox_nguoimuon.SelectedItem = "Sinh viên";
			comboBox_LoaiPhieu.SelectedItem = "Dài hạn";
			dateTimePicker_muonsach.Value = DateTime.Now;
			dateTimePicker_trasach.Value = DateTime.Now.AddDays(7);
			textBox_soluong.Text = "";
			comboBox_tensach.Focus();
		}


		private void button_sua_Click(object sender, EventArgs e)
		{
			try
			{
				if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.IsNewRow)
				{
					MessageBox.Show("Vui lòng chọn một phiếu mượn để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				int ID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
				string Tensach = this.comboBox_tensach.Text;
				string masinhvien = this.textBox_masinhvien.Text.Trim();
				string Hovaten = this.textBox_hovaten.Text.Trim();
				string Loaiphieu = this.comboBox_LoaiPhieu.Text;
				DateTime ngaymuon = dateTimePicker_muonsach.Value;
				DateTime ngaytra = dateTimePicker_trasach.Value;
				string soluongStr = this.textBox_soluong.Text.Trim();
				string masach = this.textBox_masach.Text.Trim();
				string doiTuongMuon = this.comboBox_nguoimuon.SelectedItem?.ToString();
				int isActive = Convert.ToInt32(dataGridView1.CurrentRow.Cells["IsActive"].Value);

				if (string.IsNullOrEmpty(Tensach))
				{
					MessageBox.Show("Vui lòng chọn tên sách!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBox_tensach.Focus();
					return;
				}
				if (string.IsNullOrEmpty(masach))
				{
					MessageBox.Show("Mã sách không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_masach.Focus();
					return;
				}
				if (string.IsNullOrEmpty(masinhvien))
				{
					MessageBox.Show("Mã sinh viên/giảng viên không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_masinhvien.Focus();
					return;
				}
				if (string.IsNullOrEmpty(Hovaten))
				{
					MessageBox.Show("Họ và tên không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_hovaten.Focus();
					return;
				}
				if (string.IsNullOrEmpty(doiTuongMuon))
				{
					MessageBox.Show("Vui lòng chọn đối tượng mượn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBox_nguoimuon.Focus();
					return;
				}
				if (ngaytra < ngaymuon)
				{
					MessageBox.Show("Ngày trả không thể trước ngày mượn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					dateTimePicker_trasach.Focus();
					return;
				}

				int soLuongMoiInt;
				if (!int.TryParse(soluongStr, out soLuongMoiInt) || soLuongMoiInt <= 0)
				{
					MessageBox.Show("Số lượng sách mượn phải là số nguyên dương!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox_soluong.Focus();
					return;
				}

				int soLuongCu = Convert.ToInt32(dataGridView1.CurrentRow.Cells["SoLuong"].Value);
				string maSachCu = dataGridView1.CurrentRow.Cells["MaSach"].Value.ToString();
				int chenhLechSoLuong = soLuongMoiInt - soLuongCu;


				if (chenhLechSoLuong > 0)
				{
					int soLuongSachDaMuonHienTai = modify.GetSoLuongSachDangMuon(masinhvien, doiTuongMuon) - soLuongCu;
					int gioiHanMuon = (doiTuongMuon == "Sinh viên") ? 2 : 4;

					if (soLuongSachDaMuonHienTai + soLuongMoiInt > gioiHanMuon)
					{
						MessageBox.Show($"Bạn đang có {soLuongSachDaMuonHienTai} quyển sách khác. {doiTuongMuon} chỉ được mượn tối đa {gioiHanMuon} quyển. " +
										$"Với phiếu này, bạn chỉ có thể mượn thêm tối đa {Math.Max(0, gioiHanMuon - soLuongSachDaMuonHienTai)} quyển nữa.",
										"Lỗi Giới Hạn Mượn Sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}
				}

				using (SQLiteConnection sqlConnection = Connection.GetSQLiteConnection())
				{
					sqlConnection.Open();
					if (masach != maSachCu)
					{
						string restoreOldBookQuery = "UPDATE quanlysach SET SoLuong = SoLuong + @SoLuongCu WHERE MaSach = @MaSachCu";
						SQLiteCommand restoreOldCmd = new SQLiteCommand(restoreOldBookQuery, sqlConnection);
						restoreOldCmd.Parameters.AddWithValue("@SoLuongCu", soLuongCu);
						restoreOldCmd.Parameters.AddWithValue("@MaSachCu", maSachCu);
						restoreOldCmd.ExecuteNonQuery();

						string checkNewBookQuery = "SELECT SoLuong FROM quanlysach WHERE MaSach = @MaSachMoi";
						SQLiteCommand checkNewCmd = new SQLiteCommand(checkNewBookQuery, sqlConnection);
						checkNewCmd.Parameters.AddWithValue("@MaSachMoi", masach);
						object resultNew = checkNewCmd.ExecuteScalar();
						if (resultNew == null) { MessageBox.Show("Sách mới không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
						int availableNew = Convert.ToInt32(resultNew);
						if (soLuongMoiInt > availableNew) { MessageBox.Show($"Sách mới '{Tensach}' không đủ số lượng (còn {availableNew})!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

						string updateNewBookQuery = "UPDATE quanlysach SET SoLuong = SoLuong - @SoLuongMoi WHERE MaSach = @MaSachMoi";
						SQLiteCommand updateNewCmd = new SQLiteCommand(updateNewBookQuery, sqlConnection);
						updateNewCmd.Parameters.AddWithValue("@SoLuongMoi", soLuongMoiInt);
						updateNewCmd.Parameters.AddWithValue("@MaSachMoi", masach);
						updateNewCmd.ExecuteNonQuery();
					}
					else
					{
						if (chenhLechSoLuong != 0)
						{
							if (chenhLechSoLuong > 0)
							{
								string checkQuery = "SELECT SoLuong FROM quanlysach WHERE MaSach = @MaSach";
								SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, sqlConnection);
								checkCmd.Parameters.AddWithValue("@MaSach", masach);
								object availableResult = checkCmd.ExecuteScalar();
								if (availableResult == null) { MessageBox.Show("Sách không tồn tại để kiểm tra số lượng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
								int availableQuantity = Convert.ToInt32(availableResult);
								if (chenhLechSoLuong > availableQuantity)
								{
									MessageBox.Show($"Sách '{Tensach}' không đủ số lượng để mượn thêm (còn {availableQuantity}, cần thêm {chenhLechSoLuong}).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
									return;
								}
							}
							string updateQuery = "UPDATE quanlysach SET SoLuong = SoLuong - @ChenhLechSoLuong WHERE MaSach = @MaSach";
							SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, sqlConnection);
							updateCmd.Parameters.AddWithValue("@ChenhLechSoLuong", chenhLechSoLuong);
							updateCmd.Parameters.AddWithValue("@MaSach", masach);
							updateCmd.ExecuteNonQuery();
						}
					}
				}


				muontra muontra = new muontra(ID, Tensach, masinhvien, Hovaten, Loaiphieu, ngaymuon, ngaytra, soluongStr, masach, doiTuongMuon, isActive);
				if (modify.suaphieu(muontra))
				{
					if (chenhLechSoLuong > 0)
					{
						modify.CapNhatThongKeMuon(masinhvien, Hovaten, chenhLechSoLuong, ngaymuon);
					}
					else if (chenhLechSoLuong < 0)
					{
						modify.CapNhatThongKeTra(masinhvien, Hovaten, Math.Abs(chenhLechSoLuong), DateTime.Now);
					}

					MessageBox.Show("Sửa phiếu mượn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					LoadDataGridView();
					ClearInputFields();
				}
				else
				{
					MessageBox.Show("Sửa phiếu mượn thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khi sửa phiếu mượn: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void button_trasach_Click(object sender, EventArgs e)
		{
			try
			{
				if (dataGridView1.SelectedRows.Count == 0)
				{
					MessageBox.Show("Bạn chưa chọn sách để trả", "Lỗi",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (dataGridView1.SelectedRows.Count > 1)
				{
					MessageBox.Show("Vui lòng chỉ chọn một phiếu để thực hiện trả sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
				string masinhvien = dataGridView1.SelectedRows[0].Cells["MaSinhVien"].Value.ToString();
				string hoVaTen = dataGridView1.SelectedRows[0].Cells["HoVaTen"].Value.ToString();
				string maSachTra = dataGridView1.SelectedRows[0].Cells["MaSach"].Value.ToString();
				int soLuongTra = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["SoLuong"].Value);
				DateTime ngayTraThucTe = DateTime.Now;

				Console.WriteLine($"DEBUG button_trasach_Click: Attempting to return book. MaSach='{maSachTra}', SoLuongTra='{soLuongTra}' for PhieuID='{ID}'");

				// Cập nhật số lượng sách trong bảng quanlysach
				using (SQLiteConnection sqlConnection = Connection.GetSQLiteConnection())
				{
					sqlConnection.Open();
					string updateBookQuery = "UPDATE quanlysach SET SoLuong = SoLuong + @SoLuongTra WHERE MaSach = @MaSach";
					SQLiteCommand updateBookCommand = new SQLiteCommand(updateBookQuery, sqlConnection);
					updateBookCommand.Parameters.AddWithValue("@SoLuongTra", soLuongTra);
					updateBookCommand.Parameters.AddWithValue("@MaSach", maSachTra);
					int rowsAffectedOnReturn = updateBookCommand.ExecuteNonQuery(); // Capture rows affected
					Console.WriteLine($"DEBUG button_trasach_Click: Updated SoLuong in quanlysach for MaSach='{maSachTra}' by +{soLuongTra}. Rows affected: {rowsAffectedOnReturn}");
				}

				if (modify.trasach(ID)) // This updates muontrasach table (IsActive=0, LoaiPhieu="Đã trả")
				{
					modify.CapNhatThongKeTra(masinhvien, hoVaTen, soLuongTra, ngayTraThucTe);
					MessageBox.Show("Trả sách thành công", "Thông báo",
						MessageBoxButtons.OK, MessageBoxIcon.Information);
					Console.WriteLine($"DEBUG button_trasach_Click: Successfully marked borrow record ID='{ID}' as returned.");
					LoadDataGridView();
					ClearInputFields();
				}
				else
				{
					Console.WriteLine($"DEBUG button_trasach_Click: Failed to update muontrasach (trasach method returned false) for ID='{ID}'. Restoring SoLuong for MaSach='{maSachTra}'.");
					// Nếu cập nhật trạng thái phiếu thất bại, khôi phục số lượng sách
					using (SQLiteConnection sqlConnection = Connection.GetSQLiteConnection())
					{
						sqlConnection.Open();
						string restoreQuery = "UPDATE quanlysach SET SoLuong = SoLuong - @SoLuongTra WHERE MaSach = @MaSach"; // Subtract back
						SQLiteCommand restoreCommand = new SQLiteCommand(restoreQuery, sqlConnection);
						restoreCommand.Parameters.AddWithValue("@SoLuongTra", soLuongTra);
						restoreCommand.Parameters.AddWithValue("@MaSach", maSachTra);
						int rowsAffectedOnRestore = restoreCommand.ExecuteNonQuery();
						Console.WriteLine($"DEBUG button_trasach_Click: Restored SoLuong in quanlysach for MaSach='{maSachTra}' by -{soLuongTra}. Rows affected: {rowsAffectedOnRestore}");
					}

					MessageBox.Show("Trả sách thất bại", "Lỗi",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR button_trasach_Click: {ex.ToString()}");
				MessageBox.Show("Lỗi khi trả sách: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}




		private void button_kiemtra_Click(object sender, EventArgs e)
		{
			string timsv = this.textBox_timsinhvien.Text.Trim();
			if (string.IsNullOrEmpty(timsv))
			{
				MessageBox.Show("Vui lòng nhập mã sinh viên/giảng viên để kiểm tra.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LoadDataGridView();
				return;
			}
			using (SQLiteConnection sqliteConnection = Connection.GetSQLiteConnection())
			{
				try
				{
					sqliteConnection.Open();
					DataTable sourceTable = modify.timsv(timsv, sqliteConnection);
					if (sourceTable != null && sourceTable.Rows.Count > 0)
					{
						DataRow[] activeRows = sourceTable.Select("IsActive = 1");
						if (activeRows.Length > 0)
						{
							dataGridView1.DataSource = activeRows.CopyToDataTable();
						}
						else
						{
							dataGridView1.DataSource = sourceTable.Clone();
							MessageBox.Show($"Không tìm thấy phiếu mượn nào đang hoạt động cho mã '{timsv}'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
					else
					{
						if (dataGridView1.DataSource is DataTable currentDt)
						{
							dataGridView1.DataSource = currentDt.Clone();
						}
						else
						{
							dataGridView1.DataSource = new DataTable();
						}
						MessageBox.Show($"Không tìm thấy thông tin mượn sách cho mã '{timsv}'.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void textBox_timsinhvien_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				button_kiemtra_Click(sender, e);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void textBox_soluong_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				button_phieumuon.Focus();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Lịch hẹn sẽ hiện ở một cửa sổ riêng, bạn muốn mở chứ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				lichhenmuonsach lichhenmuonsach = new lichhenmuonsach();
				lichhenmuonsach.ShowDialog();
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Bảng thống kê sinh viên quá hạn sẽ hiện ở một cửa sổ riêng, bạn muốn mở chứ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				thongkesinhvienquahan thongkesinhvienquahan = new thongkesinhvienquahan();
				thongkesinhvienquahan.ShowDialog();
			}
		}

		private void button_quetmamuonsach_Click(object sender, EventArgs e)
		{
			quetmamuonsach quetForm = new quetmamuonsach();
			quetForm.Owner = this;
			quetForm.ShowDialog();
		}

		private void button_quetmatrasach_Click(object sender, EventArgs e)
		{
			quetmatrasach quetForm = new quetmatrasach();
			quetForm.Owner = this;
			quetForm.ShowDialog();
		}

		public void SetBookCode(string code)
		{
			textBox_masach.Text = code;
			if (!string.IsNullOrWhiteSpace(code))
			{
				using (SQLiteConnection con = Connection.GetSQLiteConnection())
				{
					try
					{
						con.Open();
						SQLiteCommand command = new SQLiteCommand("SELECT TenSach FROM quanlysach WHERE MaSach = @MaSach", con);
						command.Parameters.AddWithValue("@MaSach", code);
						object tenSachResult = command.ExecuteScalar();
						if (tenSachResult != null)
						{
							comboBox_tensach.Text = tenSachResult.ToString();
						}
						else
						{
							comboBox_tensach.Text = "";
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show("Lỗi khi tìm tên sách từ mã sách quét: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
						comboBox_tensach.Text = "";
					}
				}
			}
			else
			{
				comboBox_tensach.Text = "";
			}
		}

		public void XuLyMaSachQuet(string maSachQuet, string maSinhVienQuet, string hoVaTenQuet)
		{
			try
			{
				LoadDataGridView();

				bool timThay = false;
				foreach (DataGridViewRow row in dataGridView1.Rows)
				{
					if (row.Cells["MaSach"] != null && row.Cells["MaSach"].Value != null && row.Cells["MaSach"].Value.ToString() == maSachQuet &&
						row.Cells["MaSinhVien"] != null && row.Cells["MaSinhVien"].Value != null && row.Cells["MaSinhVien"].Value.ToString() == maSinhVienQuet)
					{
						dataGridView1.ClearSelection();
						row.Selected = true;

						DataGridViewCell visibleCell = null;
						foreach (DataGridViewCell cell in row.Cells)
						{
							if (cell.Visible && cell.OwningColumn.Visible)
							{
								visibleCell = cell;
								break;
							}
						}

						if (visibleCell != null)
						{
							dataGridView1.CurrentCell = visibleCell;
						}

						if (dataGridView1.FirstDisplayedScrollingRowIndex > row.Index ||
							(dataGridView1.FirstDisplayedScrollingRowIndex + dataGridView1.DisplayedRowCount(true) - 1) < row.Index)
						{
							dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
						}

						timThay = true;
						break;
					}
				}

				Application.DoEvents();

				if (timThay)
				{
					if (MessageBox.Show($"Mã sinh viên: {maSinhVienQuet}, Họ và tên: {hoVaTenQuet} muốn trả sách có mã '{maSachQuet}'?",
						"Xác nhận trả sách", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						button_trasach_Click(null, null);
					}
				}
				else
				{
					MessageBox.Show($"Không tìm thấy phiếu mượn phù hợp cho sách có mã '{maSachQuet}' của sinh viên/giảng viên '{maSinhVienQuet} - {hoVaTenQuet}'.",
						"Không tìm thấy phiếu mượn", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi xử lý mã sách quét để trả: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count && !dataGridView1.Rows[e.RowIndex].IsNewRow)
			{
				DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

				textBox_masach.Text = row.Cells["MaSach"].Value?.ToString() ?? "";
				comboBox_tensach.Text = row.Cells["TenSach"].Value?.ToString() ?? "";


				textBox_masinhvien.Text = row.Cells["MaSinhVien"].Value?.ToString() ?? "";
				textBox_hovaten.Text = row.Cells["HoVaTen"].Value?.ToString() ?? "";

				string doiTuongMuonValue = row.Cells["DoiTuongMuon"].Value?.ToString();
				if (!string.IsNullOrEmpty(doiTuongMuonValue) && comboBox_nguoimuon.Items.Contains(doiTuongMuonValue))
				{
					comboBox_nguoimuon.SelectedItem = doiTuongMuonValue;
				}
				else
				{
					comboBox_nguoimuon.SelectedIndex = -1;
				}

				string loaiPhieuValue = row.Cells["LoaiPhieu"].Value?.ToString();
				if (!string.IsNullOrEmpty(loaiPhieuValue) && comboBox_LoaiPhieu.Items.Contains(loaiPhieuValue))
				{
					comboBox_LoaiPhieu.SelectedItem = loaiPhieuValue;
				}
				else
				{
					comboBox_LoaiPhieu.SelectedIndex = -1;
				}


				if (DateTime.TryParse(row.Cells["NgayMuon"].Value?.ToString(), out DateTime ngayMuon))
				{
					dateTimePicker_muonsach.Value = ngayMuon;
				}
				if (DateTime.TryParse(row.Cells["NgayTra"].Value?.ToString(), out DateTime ngayTra))
				{
					dateTimePicker_trasach.Value = ngayTra;
				}
				textBox_soluong.Text = row.Cells["SoLuong"].Value?.ToString() ?? "";
			}
		}
	}
}