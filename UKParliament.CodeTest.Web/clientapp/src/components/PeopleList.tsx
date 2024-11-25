import React, { useState } from 'react';
import '../App.css';
import { IPerson } from '../components/Interfaces';

interface PeopleListProps {
    people: IPerson[] | null;
    handleSelectedPerson: (person: IPerson) => void;
}

const PeopleList: React.FC<PeopleListProps> = ({ people, handleSelectedPerson }) => {
    const [selectedPerson, setSelectedPerson] = useState<number | null>(null);

    const displayPeopleList = () => {
        if (!Array.isArray(people) || people.length === 0) {
            return <div>Loading..</div>;
        }
        return people
            .map((person, index) => (
                <div
                    className={`list-person-entry ${person.id === selectedPerson ? 'selected' : ''}`}
                    key={person.id}
                    onClick={() => { handleSelectedPerson(person); setSelectedPerson(person.id); }}
                >
                    {person.lastName}, {person.firstName }
                </div>
            ));

    };

    return (
        <div className="list-person">
            {displayPeopleList()}
        </div>
    );
};

export default PeopleList;
