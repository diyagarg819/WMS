export interface AnnouncementRecord {
  announcementId: number;
  title: string;
  message: string;
  isActive: boolean;
  createdOn: string;
  creatorName: string;
  targetAudience: string;
}

export interface CreateAnnouncementRequest {
  title: string;
  message: string;
  targetAudience: string;
}

export interface UpdateAnnouncementRequest {
  title: string;
  message: string;
  isActive: boolean;
  targetAudience: string;
}
