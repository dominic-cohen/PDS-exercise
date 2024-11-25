import React from 'react';
import '../App.css';
import Logo from './logo.jpg';

interface TopBannerProps { 
    handleCreateNewClick: () => void;
}

const TopBanner: React.FC<TopBannerProps> = ({ handleCreateNewClick }) => {
    return (
        <div className="header">
            <div className="content">
                <img src={Logo} alt="Logo" height="45px" />
              
                <h1 style={{marginBottom: "0px"} }>UK Parliament</h1>
                <span>People editor excerise - using react & typescript
                <input type="button" value="Create New Person" className="form-button-small" disabled={false} onClick={() => handleCreateNewClick()} />
                </span>
            </div>
            <div
                style={{
                    position: "absolute",
                    top: "50px",
                    left: "0",
                    width: "100%",
                    height: "1px",
                    backgroundColor: "grey",
                }}></div>
        </div>
    );
};

export default TopBanner;
