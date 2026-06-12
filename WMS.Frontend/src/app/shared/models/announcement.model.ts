export interface AnnouncementRecord {
  announcementId: number;
  title: string;
  message: string;
  isActive: boolean;
  createdOn: string;
  creatorName: string;
  targetEmployeeIds: number[];
}

export interface CreateAnnouncementRequest {
  title: string;
  message: string;
  targetEmployeeIds: number[];
}

export interface UpdateAnnouncementRequest {
  title: string;
  message: string;
  isActive: boolean;
  targetEmployeeIds: number[];
}
