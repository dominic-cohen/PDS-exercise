
import { IDepartment, IPerson, IValidationErrors } from '../components/Interfaces';

export const fetchAllPeople = async (): Promise<IPerson[]> => {
    const response = await fetch(`api/person`);
    return await checkResponse(response);
};


export const updatePerson = async (person: IPerson): Promise<IValidationErrors> => {
    const id = person.id;
    const response = await fetch(`api/person/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(person),
    });

    if (response.ok) {
        return { validationErrors: [] };
    }

    if (response.status === 400) {
        const errorResponse = await response.json();
        if (errorResponse.errors) {
            return {
                validationErrors: errorResponse.errors.map(
                    (err: { propertyName: string }) => err.propertyName
                ),
            };
        }
    }

    console.error('Error updating person:', response.statusText);
    return { validationErrors: ['UnknownError'] };
};



export const addPerson = async (person: IPerson): Promise<IValidationErrors> => {
    const response = await fetch(`api/person`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(person),
    });

    if (response.ok) {
        const newPerson = await response.json();
        return { validationErrors: [] };
    }

    if (response.status === 400) {
        const errorResponse = await response.json();
        if (errorResponse.errors) {
            return { validationErrors: errorResponse.errors.map((err: { propertyName: string }) => err.propertyName) };
        }
    }

    console.error('Error adding person:', response.statusText);
    return { validationErrors: ['UnknownError'] };
};



export const fetchAllDepartments = async (): Promise<IDepartment[]> => {
    const response = await fetch(`api/department`);
    return await checkResponse(response);
};

const checkResponse = async (response: Response) => {
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'API request failed');
    }
    return response.json();
};


export const deletePerson = async (id: number): Promise<{ success: boolean; message?: string }> => {
    try {
        const response = await fetch(`api/person/${id}`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            return { success: true };
        }

        if (response.status === 404) {
            const error = await response.json();
            return { success: false, message: error.message || 'Person not found.' };
        }

        const error = await response.json();
        return {
            success: false,
            message: error.message || 'An error occurred while deleting the person.',
        };
    } catch (error) {
        console.error('Error deleting person:', error);
        return {
            success: false,
            message: 'A network or server error occurred.',
        };
    }
};


const errorFieldConfiguration = [
    { fieldName: "FirstName", displayName: "First Name" },
    { fieldName: "LastName", displayName: "Last Name" },
    { fieldName: "DOB", displayName: "Date of Birth" },
    { fieldName: "DepartmentId", displayName: "Department" },
];

export const collateAndTidyErrorMessages = (errors: string[]): string => {
    const uniqueErrors = [...new Set(errors)];
    const allErrors = uniqueErrors.map(error => {
        const config = errorFieldConfiguration.find(conf => conf.fieldName === error);
        return config ? config.displayName : error;
    });
    return allErrors.join(', ');
};