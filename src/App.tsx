import { Routes, Route, BrowserRouter } from "react-router-dom";
import ActivityOne from "./ActivityOne";
import ActivityTwo from "./ActivityTwo";
import HomePage from "./HomePage";

function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/ActivityOne" element={<ActivityOne />} />
          <Route path="/ActivityTwo" element={<ActivityTwo />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}
export default App;
