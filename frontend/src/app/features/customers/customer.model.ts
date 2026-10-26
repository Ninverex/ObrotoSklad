export interface Customer {
  id: number;
  name: string;
  nip: string;
  address: string;
  mail: string;
  phoneNumber: string;
}

export interface CreateCustomerRequest {
  name: string;
  nip: string;
  address: string;
  mail: string;
  phoneNumber: string;
}