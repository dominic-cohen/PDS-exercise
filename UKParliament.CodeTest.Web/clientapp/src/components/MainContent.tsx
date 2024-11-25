import React, { useEffect, useState } from 'react';
import '../App.css';
import PeopleList from './PeopleList';
import FormCreate from './FormCreate'
import FormEditable from './FormEditable'
import FormReadOnly from './FormReadOnly'
import { fetchAllPeople } from '../services/managePeopleAPI';
import { IPerson, EnumFormState } from '../components/Interfaces';


interface MainContentProps {
    createNewPressed: boolean;
    setCreateNewPressed: (pressed: boolean) => void;
}

const MainContent: React.FC<MainContentProps> = ({ createNewPressed, setCreateNewPressed }) => {
    const [people, setPeople] = useState<IPerson[]>([]);
    const [selectedPerson, setSelectedPerson] = useState<IPerson | null>(null);
    const [formState, setFormState] = useState<EnumFormState>(EnumFormState.Read);

    const fetchPeople = async () => {
        try {
            const data = await fetchAllPeople();
            setPeople(data);
        } catch (error) {
            console.error('Error fetching people:', error);
        }
    };

    useEffect(() => {
        fetchPeople();
        setFormState(EnumFormState.Read);
    }, []);

    const onSelectedPerson = (person: IPerson) => {
        setSelectedPerson(person);
        setFormState(EnumFormState.Read);
    }

    const onEditClick = (person: IPerson) => {
        setFormState(EnumFormState.Edit);
        fetchPeople();
    }

    const onSaveClick = (person: IPerson) => {
        setCreateNewPressed(false);
        setFormState(EnumFormState.Read);
        setSelectedPerson(person);
        fetchPeople();
    }

    const onDeleteClick = () => {
        setCreateNewPressed(false);
        setFormState(EnumFormState.Read);
        setSelectedPerson(null);
        fetchPeople();
    }

    const onCancelClick = (person?: IPerson) => {
        setCreateNewPressed(false);
        setFormState(EnumFormState.Read);
        if (person) {
            setSelectedPerson(person); // after cancel - still select the same record in read mode
        }
        else {
            setSelectedPerson(null); // this is fine - Cancel pressed from Create New
        }
    }

    return (
        <div className="main-content">
            <div className="content">
                <div className="form-title">People Editor</div>
                <div className="container">
                    <div className="left-column">
                        {people && (<PeopleList people={people} handleSelectedPerson={onSelectedPerson} />)}
                    </div>
                    <div className="right-column">
                        {createNewPressed ? <FormCreate handleSaveClick={onSaveClick} handleCancelClick={onCancelClick} /> : ''}
                        {!createNewPressed && formState === EnumFormState.Read ? <FormReadOnly person={selectedPerson} handleEditClick={onEditClick} /> : ''}
                        {!createNewPressed && formState === EnumFormState.Edit ? <FormEditable person={selectedPerson} handleSaveClick={onSaveClick} handleDeleteClick={onDeleteClick} handleCancelClick={onCancelClick} /> : ''}
                    </div>
                </div>
            </div>
           
        </div>
    );
};

export default MainContent;
