import React, { useState, useEffect } from 'react';
import '../App.css';
import { IPerson, IDepartment } from '../components/Interfaces';
import { fetchAllDepartments, updatePerson, deletePerson, collateAndTidyErrorMessages } from '../services/managePeopleAPI';

interface FormEditProps {
    person: IPerson | null;
    handleSaveClick: (person: IPerson) => void;
    handleDeleteClick: () => void;
    handleCancelClick: (person?: IPerson) => void;
}


const FormEditable: React.FC<FormEditProps> = ({ person, handleSaveClick, handleDeleteClick, handleCancelClick }) => {
    const [departments, setDepartments] = useState<IDepartment[]>([]);
    const [errorMessage, setErrorMessage] = useState<string >('');
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
                const msg = 'Error fetching departments';
                console.error(msg, error);
                setErrorMessage(msg);
            }
        };

        fetchDepartments();
    }, []);


    useEffect(() => {
        if (person) {
            setFormData({
                title: person.title || '',
                firstName: person.firstName || '',
                lastName: person.lastName || '',
                dob: person.dob || '',
                departmentId: person.departmentId ? person.departmentId.toString() : ''
            });
        }
    }, [person, departments]);


    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { id, value } = e.target;
        setFormData({ ...formData, [id]: value });
    };

    const onSavePerson = async (id: number) => {
        let errorMessage = '';
        const updatedPerson: IPerson = {
            id: id,
            title: formData.title,
            firstName: formData.firstName,
            lastName: formData.lastName,
            dob: formData.dob,
            departmentId: parseInt(formData.departmentId, 10) || 0
        };

        const { validationErrors } = await updatePerson(updatedPerson);
        if (validationErrors.length === 0) {
            handleSaveClick(updatedPerson);
        } else {
            errorMessage = `Please correct these field(s) : ${collateAndTidyErrorMessages(validationErrors)}`;
        }
        setErrorMessage(errorMessage);
    };

    const onDeleteClick = async (id: number) => {
        const { success, message } = await deletePerson(id);
        if (success) {
            handleDeleteClick();
        }
        setErrorMessage(message || '');
    }

    const onCancelClick = async (person: IPerson) => {
        handleCancelClick(person)
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
                    <input type="button" value="Save" className="form-button" disabled={false} onClick={() => person && onSavePerson(person.id)} />
                    <input type="button" value="Cancel" className="form-button" disabled={false} onClick={() => person && onCancelClick(person)} />
                    <input type="button" value="Delete" className="form-button" disabled={false} onClick={() => person && onDeleteClick(person.id)} />
                </div>
            </div>
        </div>
    );
};

export default FormEditable;
