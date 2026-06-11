export interface ClientRecord {
    clientId: number;
    clientName: string;
    clientAdress?: string;
    clientPhoneNumber?: number;
    clientLocation?: string;
    status: boolean;
}

export interface CreateClientRequest {
    clientName: string;
    clientAdress?: string;
    clientPhoneNumber?: number;
    clientLocation?: string;
    status: boolean;
}

export interface UpdateClientRequest {
    clientName: string;
    clientAdress?: string;
    clientPhoneNumber?: number;
    clientLocation?: string;
    status: boolean;
}
