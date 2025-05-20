﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SQLite;
using ExcelDataReader;
using QRCoder;
using System.Drawing.Imaging;
using System.IO;

namespace ungdungthuviencaocap
{
	public partial class quanlysach : DevExpress.XtraEditors.XtraForm
	{
		Modify modify;

		public quanlysach()
		{
			InitializeComponent();
			// Cho phép nhập liệu tự do vào ComboBox
			comboBox_theloai.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox_tacgia.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox_nhaxuatban.DropDownStyle = ComboBoxStyle.DropDown;
		}

		private void quanlysach_Load(object sender, EventArgs e)
		{
			try
			{
				modify = new Modify();
				LoadDataGridView();
				LoadComboBoxData();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khi tải form Quản lý sách: " + ex.Message, "Lỗi Khởi Tạo", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void LoadComboBoxData()
		{
			try
			{
				List<string> tenTheLoaiList = modify.GetAllTenTheLoai();
				comboBox_theloai.DataSource = null;
				comboBox_theloai.Items.Clear(); // Xóa item cũ trước khi thêm mới
				if (tenTheLoaiList != null) comboBox_theloai.Items.AddRange(tenTheLoaiList.ToArray());
				comboBox_theloai.Text = ""; // Để trống ban đầu

				List<string> tenTacGiaList = modify.GetAllAuthorNames();
				comboBox_tacgia.DataSource = null;
				comboBox_tacgia.Items.Clear();
				if (tenTacGiaList != null) comboBox_tacgia.Items.AddRange(tenTacGiaList.ToArray());
				comboBox_tacgia.Text = "";

				List<string> tenNXBList = modify.GetAllTenNhaXuatBan();
				comboBox_nhaxuatban.DataSource = null;
				comboBox_nhaxuatban.Items.Clear();
				if (tenNXBList != null) comboBox_nhaxuatban.Items.AddRange(tenNXBList.ToArray());
				comboBox_nhaxuatban.Text = "";
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khi tải dữ liệu cho ComboBox: " + ex.Message, "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void LoadDataGridView()
		{
			try
			{
				dataGridView1.DataSource = null;
				dataGridView1.DataSource = modify.getAllbooks();

				if (dataGridView1.Columns.Count > 0)
				{
					dataGridView1.Columns["ID"].Visible = false;
					if (dataGridView1.Columns.Contains("anh")) dataGridView1.Columns["anh"].Visible = false;
					if (dataGridView1.Columns.Contains("pdf")) dataGridView1.Columns["pdf"].Visible = false;
					if (dataGridView1.Columns.Contains("tomtatnoidung")) dataGridView1.Columns["tomtatnoidung"].Visible = false;

					dataGridView1.Columns["Masach"].HeaderText = "Mã sách";
					dataGridView1.Columns["TenSach"].HeaderText = "Tên sách";
					dataGridView1.Columns["TheLoai"].HeaderText = "Thể loại";
					dataGridView1.Columns["TacGia"].HeaderText = "Tác giả";
					dataGridView1.Columns["SoLuong"].HeaderText = "Số lượng";
					dataGridView1.Columns["NhaXuatBan"].HeaderText = "Nhà xuất bản";
					dataGridView1.Columns["NamXuatBan"].HeaderText = "Năm xuất bản";

					dataGridView1.Columns["Masach"].DisplayIndex = 0;
					dataGridView1.Columns["TenSach"].DisplayIndex = 1;
					dataGridView1.Columns["TheLoai"].DisplayIndex = 2;
					dataGridView1.Columns["TacGia"].DisplayIndex = 3;
					dataGridView1.Columns["NhaXuatBan"].DisplayIndex = 4;
					dataGridView1.Columns["NamXuatBan"].DisplayIndex = 5;
					dataGridView1.Columns["SoLuong"].DisplayIndex = 6;

					dataGridView1.ReadOnly = true;
					dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
					dataGridView1.AllowUserToAddRows = false;
					dataGridView1.MultiSelect = true;
					dataGridView1.RowHeadersVisible = false;
					dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
					dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
				}
			}
			catch (ArgumentException argEx)
			{
				MessageBox.Show($"Lỗi cấu hình cột DataGridView: {argEx.Message}\nKiểm tra lại tên cột trong CSDL và câu lệnh SELECT.", "Lỗi Cấu Hình Grid", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khi tải dữ liệu vào GridView: " + ex.Message, "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button_themsach_Click(object sender, EventArgs e)
		{
			try
			{
				string Tensach = this.textBox_tensach.Text.Trim();
				string TheloaiInput = this.comboBox_theloai.Text.Trim();
				string TacgiaInput = this.comboBox_tacgia.Text.Trim();
				string NhaxuatbanInput = this.comboBox_nhaxuatban.Text.Trim();
				int soluong;
				int Namxuatban;

				if (string.IsNullOrWhiteSpace(Tensach)) { MessageBox.Show("Tên sách không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox_tensach.Focus(); return; }
				if (string.IsNullOrWhiteSpace(TheloaiInput)) { MessageBox.Show("Thể loại không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); comboBox_theloai.Focus(); return; }
				if (string.IsNullOrWhiteSpace(TacgiaInput)) { MessageBox.Show("Tên tác giả không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); comboBox_tacgia.Focus(); return; }
				if (string.IsNullOrWhiteSpace(NhaxuatbanInput)) { MessageBox.Show("Tên nhà xuất bản không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); comboBox_nhaxuatban.Focus(); return; }
				if (!int.TryParse(this.textBox_soluong.Text, out soluong) || soluong < 0) { MessageBox.Show("Số lượng phải là một số nguyên không âm.", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox_soluong.Focus(); return; }
				if (!int.TryParse(this.textBox_namxuatban.Text, out Namxuatban) || Namxuatban <= 1000 || Namxuatban > DateTime.Now.Year + 1) { MessageBox.Show($"Năm xuất bản không hợp lệ. Phải là số nguyên (ví dụ: 1990 - {DateTime.Now.Year + 1}).", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox_namxuatban.Focus(); return; }

				// Tự động thêm nếu chưa có
				modify.AddTheLoaiIfNotExists(TheloaiInput);
				modify.AddTacGiaIfNotExists(TacgiaInput); // Giả sử bạn đã tạo hàm này trong Modify.cs
				modify.AddNhaXuatBanIfNotExists(NhaxuatbanInput);


				sachquanly newSach = new sachquanly(0, null, Tensach, TheloaiInput, TacgiaInput, soluong, NhaxuatbanInput, Namxuatban);
				if (modify.Insert(newSach))
				{
					MessageBox.Show("Thêm sách thành công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					LoadDataGridView();
					LoadComboBoxData(); // Tải lại ComboBox để có thể có item mới
					clear();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi không mong muốn khi thêm sách: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi thêm sách: {ex.StackTrace}");
			}
		}

		private void button_suathongtin_Click(object sender, EventArgs e)
		{
			try
			{
				if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.IsNewRow)
				{
					MessageBox.Show("Vui lòng chọn một sách cần sửa từ danh sách.", "Chưa Chọn Sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				int ID = 0;
				string Masach = null;

				if (dataGridView1.CurrentRow.Cells["ID"]?.Value != null &&
					int.TryParse(dataGridView1.CurrentRow.Cells["ID"].Value.ToString(), out ID))
				{
					Masach = dataGridView1.CurrentRow.Cells["Masach"]?.Value?.ToString();
				}
				else
				{
					MessageBox.Show("Không thể lấy ID sách hợp lệ từ dòng được chọn.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (string.IsNullOrWhiteSpace(Masach))
				{
					MessageBox.Show("Không thể lấy Mã sách hợp lệ từ dòng được chọn.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				string Tensach = this.textBox_tensach.Text.Trim();
				string TheloaiInput = this.comboBox_theloai.Text.Trim();
				string TacgiaInput = this.comboBox_tacgia.Text.Trim();
				string NhaxuatbanInput = this.comboBox_nhaxuatban.Text.Trim();

				if (string.IsNullOrWhiteSpace(Tensach)) { MessageBox.Show("Tên sách không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox_tensach.Focus(); return; }
				if (string.IsNullOrWhiteSpace(TheloaiInput)) { MessageBox.Show("Thể loại không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); comboBox_theloai.Focus(); return; }
				if (string.IsNullOrWhiteSpace(TacgiaInput)) { MessageBox.Show("Tên tác giả không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); comboBox_tacgia.Focus(); return; }
				if (string.IsNullOrWhiteSpace(NhaxuatbanInput)) { MessageBox.Show("Tên nhà xuất bản không được để trống.", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning); comboBox_nhaxuatban.Focus(); return; }


				int soluong;
				if (!int.TryParse(this.textBox_soluong.Text, out soluong) || soluong < 0) { MessageBox.Show("Số lượng phải là một số nguyên không âm.", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox_soluong.Focus(); return; }

				int Namxuatban;
				if (!int.TryParse(this.textBox_namxuatban.Text, out Namxuatban) || Namxuatban <= 1000 || Namxuatban > DateTime.Now.Year + 1) { MessageBox.Show($"Năm xuất bản không hợp lệ. Phải là số nguyên (ví dụ: 1990 - {DateTime.Now.Year + 1}).", "Dữ Liệu Không Hợp Lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox_namxuatban.Focus(); return; }

				// Tự động thêm nếu chưa có
				modify.AddTheLoaiIfNotExists(TheloaiInput);
				modify.AddTacGiaIfNotExists(TacgiaInput);
				modify.AddNhaXuatBanIfNotExists(NhaxuatbanInput);

				sachquanly updatedSach = new sachquanly(ID, Masach, Tensach, TheloaiInput, TacgiaInput, soluong, NhaxuatbanInput, Namxuatban);

				if (modify.Update(updatedSach))
				{
					MessageBox.Show("Sửa thông tin sách thành công!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
					LoadDataGridView();
					LoadComboBoxData(); // Tải lại ComboBox để có thể có item mới
					clear();
				}
				else
				{
					MessageBox.Show("Không có thay đổi nào được thực hiện hoặc không tìm thấy sách để cập nhật.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi không mong muốn khi sửa thông tin sách: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi sửa sách: {ex.StackTrace}");
			}
		}

		private void button_xoasach_Click(object sender, EventArgs e)
		{
			try
			{
				if (dataGridView1.SelectedRows.Count == 0)
				{
					MessageBox.Show("Vui lòng chọn ít nhất một sách để xóa.", "Chưa Chọn Sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				string confirmMessage = dataGridView1.SelectedRows.Count == 1
					? "Bạn có chắc chắn muốn xóa sách đang chọn?"
					: $"Bạn có chắc chắn muốn xóa {dataGridView1.SelectedRows.Count} sách đã chọn?";

				DialogResult result = MessageBox.Show(confirmMessage, "Xác Nhận Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

				if (result == DialogResult.Yes)
				{
					int successCount = 0;
					List<string> failedBooks = new List<string>();
					List<int> idsToDelete = new List<int>();
					List<string> bookNamesToDelete = new List<string>();

					foreach (DataGridViewRow row in dataGridView1.SelectedRows)
					{
						if (row.IsNewRow) continue;
						if (row.Cells["ID"]?.Value != null && int.TryParse(row.Cells["ID"].Value.ToString(), out int id))
						{
							idsToDelete.Add(id);
							bookNamesToDelete.Add(row.Cells["TenSach"]?.Value?.ToString() ?? $"ID {id}");
						}
					}

					for (int i = 0; i < idsToDelete.Count; i++)
					{
						if (modify.delete(idsToDelete[i]))
						{
							successCount++;
						}
						else
						{
							failedBooks.Add(bookNamesToDelete[i]);
						}
					}

					string message = "";
					if (successCount > 0) { message += $"Đã xóa thành công {successCount} sách.\n"; }
					if (failedBooks.Count > 0)
					{
						message += $"Không thể xóa {failedBooks.Count} sách: {string.Join(", ", failedBooks)}.";
						MessageBox.Show(message, "Kết Quả Xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else if (successCount > 0)
					{
						MessageBox.Show(message.Trim(), "Kết Quả Xóa", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
					{
						MessageBox.Show("Không thể xóa sách đã chọn.", "Lỗi Xóa Sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					LoadDataGridView();
					clear();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi không mong muốn khi xóa sách: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi xóa sách: {ex.StackTrace}");
			}
		}

		public void clear()
		{
			textBox_tensach.Text = "";
			comboBox_theloai.Text = ""; // Xóa text đã nhập
			comboBox_tacgia.Text = "";
			comboBox_nhaxuatban.Text = "";
			comboBox_theloai.SelectedIndex = -1; // Bỏ chọn item
			comboBox_tacgia.SelectedIndex = -1;
			comboBox_nhaxuatban.SelectedIndex = -1;
			textBox_soluong.Text = "";
			textBox_namxuatban.Text = "";
			textBox_timkiem.Text = "";
			dataGridView1.ClearSelection();
			textBox_tensach.Focus();
		}

		private void button_nhaplai_Click(object sender, EventArgs e)
		{
			clear();
			LoadDataGridView();
			LoadComboBoxData();
		}

		private void button_nhapdulieu_Click(object sender, EventArgs e)
		{
			try
			{
				using (OpenFileDialog openFileDialog = new OpenFileDialog())
				{
					openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
					openFileDialog.Title = "Chọn file Excel chứa dữ liệu sách";
					openFileDialog.CheckFileExists = true;
					openFileDialog.CheckPathExists = true;

					if (openFileDialog.ShowDialog(this) == DialogResult.OK)
					{
						string filePath = openFileDialog.FileName;
						this.Cursor = Cursors.WaitCursor;

						try
						{
							List<sachquanly> booksFromExcel = modify.ImportFromExcel(filePath);

							if (booksFromExcel == null) { this.Cursor = Cursors.Default; return; }
							if (booksFromExcel.Count == 0) { MessageBox.Show("Không đọc được dữ liệu hợp lệ nào từ file Excel hoặc file trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information); this.Cursor = Cursors.Default; return; }

							int addedCount = 0;
							int skippedCount = 0;
							int errorCount = 0;
							StringBuilder processDetails = new StringBuilder();
							processDetails.AppendLine("--- Chi tiết xử lý nhập liệu ---");

							foreach (var book in booksFromExcel)
							{
								if (!string.IsNullOrWhiteSpace(book.Theloai))
								{
									modify.AddTheLoaiIfNotExists(book.Theloai);
								}

								if (!string.IsNullOrWhiteSpace(book.Nhaxuatban))
								{
									modify.AddNhaXuatBanIfNotExists(book.Nhaxuatban);
								}
								if (!string.IsNullOrWhiteSpace(book.Tacgia))
								{
									modify.AddTacGiaIfNotExists(book.Tacgia);
								}


								if (modify.CheckDuplicateBook(book))
								{
									skippedCount++;
									processDetails.AppendLine($"- Bỏ qua (Trùng): {book.Tensach}");
								}
								else
								{
									if (modify.Insert(book))
									{
										addedCount++;
										processDetails.AppendLine($"- Đã thêm: {book.Tensach} (Mã: {book.Masach})");
									}
									else
									{
										errorCount++;
										processDetails.AppendLine($"- Lỗi thêm: {book.Tensach}");
									}
								}
							}
							LoadDataGridView();
							LoadComboBoxData();

							string message = $"Nhập dữ liệu từ file Excel hoàn tất.\n\n" +
											 $"- Số sách mới được thêm: {addedCount}\n" +
											 $"- Số sách bị bỏ qua (đã tồn tại): {skippedCount}\n" +
											 $"- Số sách gặp lỗi khi thêm: {errorCount}";

							if (booksFromExcel.Count > 0)
							{
								const int detailThreshold = 50;
								if (booksFromExcel.Count < detailThreshold)
								{
									message += "\n\n" + processDetails.ToString();
								}
								else
								{
									try
									{
										string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "import_log.txt");
										File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Import từ '{filePath}'\n{processDetails.ToString()}\n\n");
										message += $"\n\n(Chi tiết xử lý đã được ghi vào file {logFilePath})";
									}
									catch (Exception logEx) { Console.WriteLine("Không thể ghi file log nhập liệu: " + logEx.Message); }
								}
							}
							MessageBoxIcon icon = (errorCount > 0 || skippedCount > 0) ? MessageBoxIcon.Warning : MessageBoxIcon.Information;
							MessageBox.Show(message, "Kết quả Nhập liệu", MessageBoxButtons.OK, icon);
						}
						catch (Exception ex)
						{
							MessageBox.Show("Lỗi trong quá trình xử lý dữ liệu từ Excel: " + ex.Message, "Lỗi Xử Lý", MessageBoxButtons.OK, MessageBoxIcon.Error);
							Console.WriteLine($"Stack Trace lỗi xử lý Excel: {ex.StackTrace}");
						}
						finally
						{
							this.Cursor = Cursors.Default;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi mở hộp thoại chọn file: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi mở file dialog: {ex.StackTrace}");
			}
		}


		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				string searchKeyword = this.textBox_timkiem.Text.Trim();
				if (string.IsNullOrWhiteSpace(searchKeyword))
				{
					LoadDataGridView();
					return;
				}
				this.Cursor = Cursors.WaitCursor;
				DataTable searchResult = modify.searchBooks(searchKeyword);
				dataGridView1.DataSource = searchResult;
				this.Cursor = Cursors.Default;

				if (searchResult == null || searchResult.Rows.Count == 0)
				{
					MessageBox.Show("Không tìm thấy sách nào phù hợp với từ khóa '" + searchKeyword + "'.", "Không Tìm Thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{
				this.Cursor = Cursors.Default;
				MessageBox.Show($"Lỗi khi tìm kiếm sách: {ex.Message}", "Lỗi Tìm Kiếm", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi tìm kiếm: {ex.StackTrace}");
			}
		}

		private void textBox_timkiem_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				button1_Click(sender, e);
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count && !dataGridView1.Rows[e.RowIndex].IsNewRow)
				{
					DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
					textBox_tensach.Text = row.Cells["Tensach"]?.Value?.ToString() ?? "";

					// Khi click vào DataGridView, set Text cho ComboBoxes
					comboBox_theloai.Text = row.Cells["TheLoai"]?.Value?.ToString() ?? "";
					comboBox_tacgia.Text = row.Cells["TacGia"]?.Value?.ToString() ?? "";
					comboBox_nhaxuatban.Text = row.Cells["NhaXuatBan"]?.Value?.ToString() ?? "";

					textBox_soluong.Text = row.Cells["SoLuong"]?.Value?.ToString() ?? row.Cells["soluong"]?.Value?.ToString() ?? "";
					textBox_namxuatban.Text = row.Cells["NamXuatBan"]?.Value?.ToString() ?? "";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Lỗi khi hiển thị dữ liệu sách lên form: " + ex.Message, "Lỗi Hiển Thị", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi cell click: {ex.StackTrace}");
			}
		}

		private void button_timvitrisach_Click(object sender, EventArgs e)
		{
			try
			{
				if (MessageBox.Show("Chức năng cấu hình vị trí sách sẽ mở một cửa sổ mới. Bạn có muốn tiếp tục?", "Xác Nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					vitrisach vitrisachForm = new vitrisach();
					vitrisachForm.ShowDialog(this);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Lỗi khi mở form cấu hình vị trí sách: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi mở form vị trí: {ex.StackTrace}");
			}
		}

		private void xuatmaqr_Click(object sender, EventArgs e)
		{
			try
			{
				using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
				{
					folderDialog.Description = "Chọn thư mục để lưu các file mã QR";
					folderDialog.ShowNewFolderButton = true;
					folderDialog.RootFolder = Environment.SpecialFolder.MyDocuments;

					if (folderDialog.ShowDialog(this) == DialogResult.OK)
					{
						string selectedPath = folderDialog.SelectedPath;
						string qrFolderPath = Path.Combine(selectedPath, "MaQRSach");
						this.Cursor = Cursors.WaitCursor;

						try
						{
							if (!Directory.Exists(qrFolderPath))
							{
								Directory.CreateDirectory(qrFolderPath);
							}
						}
						catch (Exception dirEx)
						{
							MessageBox.Show($"Không thể tạo thư mục '{qrFolderPath}': {dirEx.Message}", "Lỗi Tạo Thư Mục", MessageBoxButtons.OK, MessageBoxIcon.Error);
							this.Cursor = Cursors.Default;
							return;
						}

						DataTable dtBooks;
						if (dataGridView1.DataSource is DataTable currentTable && currentTable.Rows.Count > 0)
						{
							dtBooks = currentTable;
							MessageBox.Show($"Sẽ xuất mã QR cho {dtBooks.Rows.Count} sách đang hiển thị.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						else
						{
							dtBooks = modify.getAllbooks();
							if (dtBooks != null && dtBooks.Rows.Count > 0)
							{
								MessageBox.Show($"Sẽ xuất mã QR cho tất cả {dtBooks.Rows.Count} sách trong cơ sở dữ liệu.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
							}
						}

						if (dtBooks == null || dtBooks.Rows.Count == 0)
						{
							MessageBox.Show("Không có dữ liệu sách để tạo mã QR.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							this.Cursor = Cursors.Default;
							return;
						}

						int successCount = 0;
						int failCount = 0;
						StringBuilder errors = new StringBuilder();
						errors.AppendLine($"--- Chi tiết lỗi xuất QR vào '{qrFolderPath}' [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ---");

						foreach (DataRow row in dtBooks.Rows)
						{
							string maSach = null;
							string tenSach = null;
							string idSach = row["ID"]?.ToString() ?? "Không xác định";

							try
							{
								maSach = row["Masach"]?.ToString();
								tenSach = row["TenSach"]?.ToString();

								if (string.IsNullOrWhiteSpace(maSach) || string.IsNullOrWhiteSpace(tenSach))
								{
									failCount++;
									errors.AppendLine($"- Bỏ qua sách ID {idSach}: Thiếu Mã sách hoặc Tên sách.");
									continue;
								}

								string sanitizedTenSach = string.Join("_", tenSach.Split(Path.GetInvalidFileNameChars()));
								const int maxFileNameLength = 100;
								if (sanitizedTenSach.Length > maxFileNameLength)
									sanitizedTenSach = sanitizedTenSach.Substring(0, maxFileNameLength);

								string fileName = $"{maSach}_{sanitizedTenSach}.png";
								string filePath = Path.Combine(qrFolderPath, fileName);

								QRCodeGenerator qrGenerator = new QRCodeGenerator();
								QRCodeData qrCodeData = qrGenerator.CreateQrCode(maSach, QRCodeGenerator.ECCLevel.Q);
								PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
								byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(10, new byte[] { 0, 0, 0 }, new byte[] { 255, 255, 255 });

								File.WriteAllBytes(filePath, qrCodeAsPngByteArr);
								successCount++;
							}
							catch (IOException ioEx)
							{
								failCount++;
								errors.AppendLine($"- Lỗi I/O khi lưu QR cho sách '{tenSach ?? $"ID {idSach}"}' (Mã: {maSach}): {ioEx.Message}");
							}
							catch (Exception exRow)
							{
								failCount++;
								errors.AppendLine($"- Lỗi tạo/lưu QR cho sách '{tenSach ?? $"ID {idSach}"}' (Mã: {maSach}): {exRow.Message}");
								Console.WriteLine($"Stack trace lỗi tạo QR: {exRow.StackTrace}");
							}
						}
						this.Cursor = Cursors.Default;

						string resultMessage = $"Xuất mã QR hoàn tất.\n\n" +
											   $"- Số mã QR đã tạo thành công: {successCount}\n" +
											   $"- Số sách bị lỗi hoặc bỏ qua: {failCount}\n\n" +
											   $"Đã lưu vào thư mục:\n{qrFolderPath}";

						if (failCount > 0)
						{
							try
							{
								string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qrcode_error_log.txt");
								File.AppendAllText(logFilePath, errors.ToString() + "\n\n");
								resultMessage += $"\n\n(Chi tiết lỗi đã được ghi vào file {logFilePath})";
							}
							catch (Exception logEx) { Console.WriteLine("Không thể ghi file log lỗi QR: " + logEx.Message); }
							MessageBox.Show(resultMessage, "Kết quả Xuất Mã QR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
						else
						{
							MessageBox.Show(resultMessage, "Kết quả Xuất Mã QR", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.Cursor = Cursors.Default;
				MessageBox.Show($"Lỗi không mong muốn khi xuất mã QR: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"Stack Trace lỗi xuất QR: {ex.StackTrace}");
			}
		}

		private void quanlysach_FormClosing(object sender, FormClosingEventArgs e)
		{
		}
	}
}
