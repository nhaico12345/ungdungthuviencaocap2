using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ungdungthuviencaocap
{
	// Định nghĩa lớp muontra để biểu diễn thông tin một phiếu mượn trả sách
	public class muontra
	{
		// Thuộc tính ID của phiếu mượn, thường là khóa chính trong CSDL
		public int ID { get; set; }
		// Thuộc tính tên sách được mượn
		public string Tensach { get; set; }
		// Thuộc tính mã sinh viên hoặc mã người mượn
		public string Masinhvien { get; set; }
		// Thuộc tính họ và tên người mượn
		public string Hovaten { get; set; }
		// Thuộc tính loại phiếu (ví dụ: Mượn, Trả)
		public string Loaiphieu { get; set; }
		// Thuộc tính ngày mượn sách
		public DateTime Ngaymuon { get; set; }
		// Thuộc tính ngày trả sách
		public DateTime Ngaytra { get; set; }
		// Thuộc tính số lượng sách mượn (dưới dạng chuỗi, có thể cần chuyển đổi sang số)
		public string Soluong { get; set; }
		// Thuộc tính mã sách
		public string Masach { get; set; }
		// Thuộc tính đối tượng mượn (ví dụ: Sinh viên, Giảng viên) - MỚI THÊM
		public string DoiTuongMuon { get; set; }
		// Thuộc tính trạng thái phiếu, mặc định là 1 (đang mượn)
		public int IsActive { get; set; }


		// Constructor mặc định
		public muontra()
		{
		}

		// Constructor đầy đủ tham số để khởi tạo đối tượng muontra
		public muontra(int id, string tensach, string masinhvien, string hovaten, string loaiphieu, DateTime ngaymuon, DateTime ngaytra, string soluong, string masach, string doiTuongMuon = "", int isActive = 1)
		{
			this.ID = id;
			this.Tensach = tensach;
			this.Masinhvien = masinhvien;
			this.Hovaten = hovaten;
			this.Loaiphieu = loaiphieu;
			this.Ngaymuon = ngaymuon;
			this.Ngaytra = ngaytra;
			this.Soluong = soluong;
			this.Masach = masach;
			this.DoiTuongMuon = doiTuongMuon; // Gán giá trị cho thuộc tính mới
			this.IsActive = isActive;
		}
	}
}
