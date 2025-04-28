import { observer } from "mobx-react-lite"
import { Route, Routes } from "react-router-dom"
import { LoginPage } from "./pages/LoginPage"
import { DashboardPage } from "./pages/DashboardPage"


const App = observer(() => {




  return (
    <Routes>
      <Route element={<DashboardPage />} path="/dashboard" />
      <Route element={<LoginPage />} path="/" />
      <Route element={<LoginPage />} path="/login" />
    </Routes>
  )
})




export default App
