import React, { useEffect, useState } from 'react';
import '../App.css';
import { IPerson, IDepartment } from '../components/Interfaces';
import { fetchAllDepartments } from '../services/managePeopleAPI';

interface FormReadOnlyProps {
    person: IPerson | null;
    handleEditClick: (person: IPerson) => void;
}



const FormReadOnly: React.FC<FormReadOnlyProps> = ({ person, handleEditClick }) => {
    const [departments, setDepartments] = useState<IDepartment[]>([]);
    const [departmentName, setDepartmentName] = useState<string | undefined>(undefined);

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

    const getDepartmentName = (p: IPerson, allDepartments: IDepartment[]): string => {
        const oneDepartment = allDepartments.find(department => department.id === p?.departmentId)
        const departmentDisp = (oneDepartment ? oneDepartment.name : '');
        return departmentDisp;
    }

   
    return (
        <div>
            <div className="form-group">
                <label>Title</label>
                <input type="text" id="title" readOnly className="read-only-input"
                value={person ? person.title : ''}/>
                <label>First Name<span className="mandatory">*</span></label>
                <input type="text" id="firstname" readOnly className="read-only-input"
                    value={person ? person.firstName : ''}/>
                <label>Last Name<span className="mandatory">*</span></label>
                <input type="text" id="lastname" readOnly className="read-only-input" 
                    value={person ? person.lastName : ''} />
                <label>Date of Birth<span className="mandatory">*</span></label>
                <input type="text" id="DOB" readOnly className="read-only-input"
                    value={person && person.dob ? new Date(person.dob).toLocaleDateString('en-GB') : ''}/>
                <label>Department<span className="mandatory">*</span></label>
                <input type="text" id="department" readOnly className="read-only-input"
                    value={person && departments ? getDepartmentName(person, departments) : ""} />
            </div>
            {person && <input type="button" value="Edit" className="form-button" disabled={false} onClick={() => person && handleEditClick(person)} />}
            {!person && <input type="button" value="Edit" className="form-button" disabled={true} />}

        </div>
    );
};

export default FormReadOnly;
