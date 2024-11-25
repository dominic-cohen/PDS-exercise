

export enum EnumFormState {
    Read,
    Create,
    Edit,
    New
}

export interface IDepartment {
    id: number;
    name: string;
}

export interface IPerson {
    id: number;
    departmentId: number;
    title?: string;
    firstName?: string;
    lastName?: string;
    dob?: string;
    addressLine1?: string;
    addressLine2?: string;
    addressLine3?: string;
    addressLine4?: string;
    postCode?: string;
}

export interface IValidationErrors {
    validationErrors: string[];
}

