import React, { useState } from 'react'; 
import './App.css';
import TopBanner from "./components/TopBanner";
import MainContent from './components/MainContent';

const App: React.FC = () => {
    const [createNewButtonPressed, setCreateNewButtonPressed] = useState<boolean>(false);

    const onCreateNew = () => {
        setCreateNewButtonPressed(true);
    }

    return (
        <div>
            <TopBanner
                handleCreateNewClick={onCreateNew} />
            <MainContent
                createNewPressed={createNewButtonPressed}
                setCreateNewPressed={setCreateNewButtonPressed} /> 
        </div>
    );
};

export default App;
