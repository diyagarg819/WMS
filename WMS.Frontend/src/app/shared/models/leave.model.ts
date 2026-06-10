export interface LeaveRecord {
  leaveId: number;
  empId: number;
  employeeName?: string;
  leaveType: string;
  reason?: string;
  fromDate: string;
  toDate: string;
  status: string;
  appliedOn: string;
  approvedBy?: number;
  approvedOn?: string;
}

export interface ApplyLeaveRequest {
  leaveType: string;
  reason?: string;
  fromDate: string;
  toDate: string;
}

export interface UpdateLeaveStatusRequest {
  status: string;
}

export interface LeaveFilter {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  status?: string;
}
