import { Input } from '@heroui/input';
import { Button } from "@heroui/button"
import { Card, CardHeader, CardBody, CardFooter } from "@heroui/card";
import { Spacer } from "@heroui/spacer";
import { Spinner } from "@heroui/spinner";
import { Alert } from "@heroui/alert";
import { useState } from 'react';

interface LoginFormProps {
  onLogin: (email: string, password: string) => void;
  isLoading: boolean
  errorMessage: string | null
}

const LoginForm: React.FC<LoginFormProps> = ({ onLogin, isLoading, errorMessage }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onLogin(email, password);
  };

  return (
    <Card className="max-w-2xl mx-auto mt-20">
      <CardHeader>
        <span className="text-xl font-bold">Access to admin</span>
        {errorMessage &&
          <Alert color="danger" title={errorMessage} />
        }
      </CardHeader>
      <CardBody>
        <form onSubmit={handleSubmit} className="flex flex-col md:flex-row items-center">
          <div className="flex-shrink-0 mb-4 md:mb-0 md:mr-6">
            <img
              src="/orion_logo.png"
              alt="Orion"
              className="w-32 h-32 object-contain"
            />
          </div>
          <div className="flex-grow w-full space-y-4">
            <Input
              type="tezt"
              label="Username"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
            <Input
              type="password"
              label="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
            <Spacer y={2} />
            <Button type="submit" color="primary" fullWidth>
              Access
            </Button>
          </div>
        </form>
      </CardBody>
      <CardFooter className="flex flex-col items-center gap-2">
        {isLoading && (
          <div className="flex items-center gap-2">
            <Spinner size="sm" />
            <span className="text-sm">Loading....</span>
          </div>
        )}
      </CardFooter>
    </Card>
  );

};

export default LoginForm;
