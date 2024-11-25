import React, { useState, useEffect } from 'react';
import '../App.css';
import { IPerson, IDepartment } from './Interfaces';
import { fetchAllDepartments, addPerson, collateAndTidyErrorMessages } from '../services/managePeopleAPI';

interface FormCreateProps {
    handleSaveClick: (person: IPerson) => void;
    handleCancelClick: (person? : IPerson) => void;
}


const FormCreate: React.FC<FormCreateProps> = ({ handleSaveClick, handleCancelClick }) => {
    const [departments, setDepartments] = useState<IDepartment[]>([]);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const [person, setPerson] = useState<IPerson | null>(null);

    const [formData, setFormData] = useState({
        title: '',
        firstName: '',
        lastName: '',
        dob: '',
        departmentId: ''
    });

    useEffect(() => {
        const fetchDepartments = async () => {
            try {
                const data = await fetchAllDepartments();
                setDepartments(data);
            } catch (error) {
                console.error('Error fetching departments:', error);
            }
        };

        fetchDepartments();
    }, []);


    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { id, value } = e.target;
        setFormData({ ...formData, [id]: value });
    };

    const onSaveClick = async () => {
        let message = '';
        const newPerson: IPerson = {
            id: 0,
            title: formData.title,
            firstName: formData.firstName,
            lastName: formData.lastName,
            dob: formData.dob,
            departmentId: parseInt(formData.departmentId, 10) || 0 
        };
        const { validationErrors } = await addPerson(newPerson);

        if (validationErrors.length > 0) {
            message = `Please correct these field(s) : ${collateAndTidyErrorMessages(validationErrors)}`;
        }
        else {
            handleSaveClick(newPerson);
        }

        setErrorMessage(message);
      
       
    };

    const onCancelClick = async () => {
        handleCancelClick();
    }

    return (
        <div>
            <div className="form-group">
                <label>Title</label>
                <input type="text" id="title" value={formData.title} onChange={handleChange} />
                <label>First Name<span className="mandatory">*</span></label>
                <input type="text" id="firstName" value={formData.firstName} onChange={handleChange} />
                <label>Last Name<span className="mandatory">*</span></label>
                <input type="text" id="lastName" value={formData.lastName} onChange={handleChange} />
                <label>Date of Birth<span className="mandatory">*</span></label>
                <input type="date" id="dob" value={formData.dob} onChange={handleChange} />
                <label>Department<span className="mandatory">*</span></label>
                <select className="edit-dropdown"
                    id="departmentId"
                    value={formData.departmentId}
                    onChange={handleChange}
                >
                    <option value="">Select..</option>
                    {departments.map((dept) => (
                        <option key={dept.id} value={dept.id}>
                            {dept.name}
                        </option>
                    ))}
                </select>
                <span className="form-error-message">{errorMessage}</span>
                <div className="buttonGroup">
                    <input type="button" value="Save" className="form-button" disabled={false} onClick={() => onSaveClick()} />
                    <input type="button" value="Cancel" className="form-button" disabled={false} onClick={() => onCancelClick()} />
                </div>
            </div>
        
        </div>
    );
};

export default FormCreate;
