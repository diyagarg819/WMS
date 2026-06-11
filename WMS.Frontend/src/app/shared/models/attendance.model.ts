export interface AttendanceRecord {
  attendanceId: number;
  empId: number;
  employeeName?: string;
  checkIn: string;
  checkOut?: string;
  totalHours?: number;
  workMode?: string;
  attendanceDate: string;
}

export interface CheckInRequest {
  workMode?: string;
}

export interface AttendanceFilter {
  fromDate?: string;
  toDate?: string;
  searchTerm?: string;
}
